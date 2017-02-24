using System;
using System.Net.Http;

namespace Deveel.Web.Client {
	public static class HttpClientExtensions {
		public static IRestClient AsRestClient(this HttpClient client, IRestClientSettings settings) {
			return new RestClient(client, settings);
		}

		public static IRestClient AsRestClient(this HttpClient client, Action<IClientSettingsBuilder> settings) {
			return AsRestClient(client, settings, new DefaultBuildContext());
		}

		public static IRestClient AsRestClient(this HttpClient client, Action<IClientSettingsBuilder> settings, IBuildContext context) {
			var builder = new ClientSettingsBuilder();
			settings(builder);

			return client.AsRestClient(builder.Build(context));
		}
	}
}