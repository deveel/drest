using System;
using System.IO;
using System.Text;

namespace Deveel.Web.Client {
	public class ClientRequestFormatter : IRequestFormatter {
		private readonly IRestClient client;

		public ClientRequestFormatter(IRestClient client) {
			this.client = client;
		}

		public string Format(IRestRequest request) {
			var uri = UriHelper.MakeUri(client.Settings.BaseUri, request);

			var sb = new StringBuilder();

			using (var writer = new StringWriter(sb)) {
				writer.Flush();
			}

			return sb.ToString();
		}
	}
}