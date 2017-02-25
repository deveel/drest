using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public static class HttpClientExtensions {
		public static Task<HttpResponseMessage> SendAsync(this IHttpClient client, HttpRequestMessage message) {
			return client.SendAsync(message, CancellationToken.None);
		}

		public static IRestClient AsRestClient(this IHttpClient client, IRestClientSettings settings) {
			return new RestClient(client, settings);
		}

		public static IRestClient AsRestClient(this IHttpClient client, Action<IClientSettingsBuilder> settings) {
			return AsRestClient(client, settings, new DefaultBuildContext());
		}

		public static IRestClient AsRestClient(this IHttpClient client, Action<IClientSettingsBuilder> settings,
			IBuildContext context) {
			var builder = new ClientSettingsBuilder();
			settings(builder);

			return client.AsRestClient(builder.Build(context));
		}

		public static IRestClient AsRestClient(this HttpClient client, IRestClientSettings settings) {
			return new DefaultHttpClient(client).AsRestClient(settings);
		}

		public static IRestClient AsRestClient(this HttpClient client, Action<IClientSettingsBuilder> settings) {
			return new DefaultHttpClient(client).AsRestClient(settings);
		}

		public static IRestClient AsRestClient(this HttpClient client, Action<IClientSettingsBuilder> settings,
			IBuildContext context) {
			return new DefaultHttpClient(client).AsRestClient(settings, context);
		}
	}
}