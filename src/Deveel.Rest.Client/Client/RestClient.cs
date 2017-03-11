using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public class RestClient : IRestClient {
		public RestClient(IRestClientSettings settings)
			: this(CreateClient(settings), settings) {
		}

		public RestClient(IHttpClient client, IRestClientSettings settings) {
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			if (client == null)
				throw new ArgumentNullException(nameof(client));

			if (settings.BaseUri != null) {
				client.BaseAddress = settings.BaseUri;
			} else if (client.BaseAddress != null) {
				settings.BaseUri = client.BaseAddress;
			} else {
				throw new ArgumentException("A base URI is required.");
			}

			if (settings.DefaultHeaders != null) {
				foreach (var header in settings.DefaultHeaders) {
					client.AddDefaultHeader(header.Key, header.Value);
				}
			}

			Settings = settings;
			HttpClient = client;
		}

		public IRestClientSettings Settings { get; }

		protected IHttpClient HttpClient { get; }

		private static IHttpClient CreateClient(IRestClientSettings settings) {
			return settings.MessageHandler != null ? new DefaultHttpClient(settings.MessageHandler) : new DefaultHttpClient(); 
		}

		async Task<IRestResponse> IRestClient.RequestAsync(IRestRequest request, CancellationToken cancellationToken) {
			return await RequestAsync((RestRequest) request, cancellationToken);
		}

		public async Task<RestResponse> RequestAsync(RestRequest request, CancellationToken cancellationToken = default(CancellationToken)) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			var authenticate = Settings.AuthenticateRequests;
			if (request.Authenticate != null)
				authenticate = request.Authenticate.Value;

			if (authenticate) {
				if (request.Authenticator != null) {
					request.Authenticator.AuthenticateRequest(this, request);
				} else if (Settings.Authenticator != null) {
					Settings.Authenticator.AuthenticateRequest(this, request);
				} else {
					throw new AuthenticateException($"The request {request.Method} {request.Resource} requires authentication but no authenticator was set");
				}
			}

			var httpRequest = request.AsHttpRequestMessage(this);

			if (Settings.RequestHandlers != null) {
				foreach (var handler in Settings.RequestHandlers) {
					await handler.HandleRequestAsync(this, request);
				}
			}

			var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken);
			var response = new RestResponse(this, request, httpResponse);

			if (Settings.ResponseHandlers != null) {
				foreach (var handler in Settings.ResponseHandlers) {
					 await handler.HandleResponseAsync(this, response);
				}
			}

			return response;
		}

		void IRestClient.HandleFailResponse(IRestResponse response) {
			OnFailResponse(response);
		}

		protected internal virtual void OnFailResponse(IRestResponse response) {
			throw response.GetException();
		}

		public static RestClient Build(Action<IClientSettingsBuilder> builder) {
			var settings = new ClientSettingsBuilder();
			builder(settings);

			return new RestClient(settings.Build());
		}

		public static RestClient Build(Uri baseUri) {
			return Build(builder => builder.BaseUri(baseUri));
		}

		public static RestClient Build(string baseUri) {
			Uri uri;
			if (!Uri.TryCreate(baseUri, UriKind.Absolute, out uri))
				throw new ArgumentException("Invalid base URI");

			return Build(uri);
		}
	}
}