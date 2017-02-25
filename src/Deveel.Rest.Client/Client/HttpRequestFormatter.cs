using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Deveel.Web.Client {
	public class HttpRequestFormatter : IRequestFormatter {
		private readonly IRestClient client;

		public HttpRequestFormatter(IRestClient client) {
			this.client = client;
		}

		public const int HttpDefaultPort = 80;

		public string Format(IRestRequest request) {
			var httpRequest = request.AsHttpRequestMessage(client);
			var httpVersion = httpRequest.Version.ToString(2);
			var uri = httpRequest.RequestUri;
			var host = $"{uri.Host}{(uri.Port != HttpDefaultPort ? uri.Port.ToString() : String.Empty)}";

			var sb = new StringBuilder();

			using (var writer = new StringWriter(sb)) {
				writer.WriteLine($"{httpRequest.Method} {uri.PathAndQuery} / HTTP/{httpVersion}");
				writer.WriteLine($"Host: {host}");

				if (request.HasHeaders()) {
					writer.WriteLine();

					foreach (var header in request.Headers()) {
						writer.WriteLine($"{header.Key}: {SafeHeaderValue(header.Value)}");
					}
				}

				if (httpRequest.Content != null) {
					WriteContent(writer, httpRequest.Content);
				}

				writer.Flush();
			}

			return sb.ToString();
		}

		private void WriteContent(TextWriter writer, HttpContent content) {
			foreach (var header in content.Headers) {
				writer.WriteLine($"{header.Key}: {String.Join(";", header.Value)}");
			}

			writer.WriteLine();

			if (content is MultipartContent) {
				var multipart = (MultipartContent) content;
				var boundary = multipart.Headers.ContentType.Parameters.FirstOrDefault(x => x.Name == "boundary");

				foreach (var subContent in multipart) {
					writer.WriteLine(boundary.Value);
					WriteContent(writer, subContent);
					writer.WriteLine();
				}
			} else if (content is StreamContent) {
				writer.WriteLine("[binary data]");
			} else if (content is StringContent) {
				var stringContent = (StringContent) content;
				writer.WriteLine(stringContent.ReadAsStringAsync().Result);
			}
		}

		private static string SafeHeaderValue(object value) {
			return value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);
		}
	}
}