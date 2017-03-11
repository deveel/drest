using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public sealed class RestResponse : IRestResponse {
		internal RestResponse(RestClient client, RestRequest request, HttpResponseMessage response) {
			Client = client;
			Request = request;
			Response = response;
		}

		private RestClient Client { get; }

		public RestRequest Request { get; }

		IRestRequest IRestResponse.Request => Request;

		private HttpResponseMessage Response { get; }

		public HttpStatusCode StatusCode => Response.StatusCode;

		public string ReasonPhrase => Response.ReasonPhrase;

		public IDictionary<string, string> Headers => Response.Headers.ToDictionary(x => x.Key, y => SafeHeader(y.Value));

		IEnumerable<KeyValuePair<string, string>> IRestResponse.Headers => Headers;

		public string Location {
			get {
				string value;
				if (!Headers.TryGetValue("Location", out value))
					return null;

				return value;
			}
		}

		private string SafeHeader(IEnumerable<string> values) {
			return values == null ? string.Empty : String.Join(";", values);
		}

		public void AssertSuccessful() {
			if ((int) StatusCode >= 400) {
				Client.OnFailResponse(StatusCode, ReasonPhrase);
			}
		}

		public async Task<object> GetBodyAsync(CancellationToken cancellationToken) {
			if (Request.Returned == null || Request.Returned.IsVoid())
				throw new InvalidOperationException("The request has no return type: cannot extract the body");

			if (Response.Content == null)
				return null;

			if (Request.Returned.IsFile()) {
				var content = Response.Content;
				var contentType = content.Headers.ContentType != null ? content.Headers.ContentType.MediaType : null;
				return new ResponseFile(() => content.ReadAsStreamAsync(), contentType, content.Headers.ContentLength);
			} else {
				var contentType = Response.Content.Headers.ContentType.MediaType;
				var serializer =
					Client.Settings.Serializers.FirstOrDefault(
						x => x.ContentTypes.Any(y => String.Equals(y, contentType, StringComparison.OrdinalIgnoreCase)));
				if (serializer == null)
					throw new NotSupportedException($"Could not deserialize an result with Content-Type {contentType}");

				cancellationToken.ThrowIfCancellationRequested();

				var content = await Response.Content.ReadAsStringAsync();
				return serializer.Deserialize(Client, Request.Returned.ReturnType, content);
			}
		}
	}
}