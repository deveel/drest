using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public class RestClient : IRestClient {
		public RestClient(RestClientSettings settings)
			: this(new HttpClient(), settings) {
		}

		public RestClient(HttpClient client, RestClientSettings settings) {
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			if (client == null)
				throw new ArgumentNullException(nameof(client));

			Settings = settings;
			HttpClient = client;

			if (!String.IsNullOrWhiteSpace(Settings.BaseUri))
				HttpClient.BaseAddress = new Uri(Settings.BaseUri);

			if (HttpClient.BaseAddress == null)
				throw new ArgumentException("No base URI was set in the client or the settings");

			if (settings.DefaultHeaders != null) {
				foreach (var header in settings.DefaultHeaders) {
					HttpClient.DefaultRequestHeaders.Add(header.Key, WebUtility.UrlEncode(header.Value == null ? "" : header.Value.ToString()));
				}
			}

			if (!String.IsNullOrWhiteSpace(settings.ClientName) &&
			    settings.ClientVersion != null) {
				HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(settings.ClientName, settings.ClientVersion.ToString(3)));
			}
		}

		public RestClientSettings Settings { get; }

		IRestClientSettings IRestClient.Settings => Settings;

		protected HttpClient HttpClient { get; }

		public IRequestAuthenticator Authenticator { get; set; }

		public ICollection<IContentSerializer> Serializers => Settings.Serializers;

		public IContentSerializer JsonSerializer => Serializers.FirstOrDefault(x => x.SupportedFormat == ContentFormat.Json);

		public IContentSerializer XmlSerializer => Serializers.FirstOrDefault(x => x.SupportedFormat == ContentFormat.Xml);

		async Task<IRestResponse> IRestClient.RequestAsync(IRestRequest request, CancellationToken cancellationToken) {
			return await RequestAsync((RestRequest) request, cancellationToken);
		}

		public async Task<RestResponse> RequestAsync(RestRequest request, CancellationToken cancellationToken = default(CancellationToken)) {
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (request.Authenticate) {
				if (Authenticator == null)
					throw new AuthenticateException($"The request {request.Method} {request.Resource} required authentication but no authenticator was set");

				Authenticator.AuthenticateRequest(this, request);
			}

			var httpRequest = request.AsHttpRequestMessage(this);

			if (Settings.RequestHandlers != null) {
				foreach (var handler in Settings.RequestHandlers) {
					await handler.HandleRequestAsync(this, httpRequest);
				}
			}

			var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken);

			if (Settings.ResponseHandlers != null) {
				foreach (var handler in Settings.ResponseHandlers) {
					 await handler.HandleResponseAsync(this, httpResponse);
				}
			}

			return new RestResponse(this, request, httpResponse);
		}


		protected internal virtual void OnFailResponse(HttpStatusCode statusCode, string reasonPhrase) {
			switch (statusCode) {
				case HttpStatusCode.BadRequest:
					throw new BadRequestException(reasonPhrase);
				case HttpStatusCode.Conflict:
					throw new ConflictException(reasonPhrase);
				case HttpStatusCode.Forbidden:
					throw new ForbiddenException(reasonPhrase);
				case HttpStatusCode.NotFound:
					throw new NotFoundException(reasonPhrase);
				case HttpStatusCode.Unauthorized:
					throw new UnauthorizedException(reasonPhrase);
				default:
					throw new RestResponseException(statusCode, reasonPhrase);
			}
		}
	}
}