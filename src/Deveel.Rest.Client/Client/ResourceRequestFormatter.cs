using System;
using System.IO;
using System.Net;
using System.Text;

namespace Deveel.Web.Client {
	public class ResourceRequestFormatter : IRequestFormatter {
		public string Format(IRestRequest request) {
			var sb = new StringBuilder();

			using (var writer = new StringWriter(sb)) {
				writer.WriteLine($"{request.Method} {MakeResourceandQuery(request)}");
				foreach (var header in request.Headers()) {
					writer.WriteLine($"{header.Key}: {header.Value}");
				}

				var body = request.Body();
				if (body != null)
					WriteBody(writer, body);

				writer.Flush();
			}

			return sb.ToString();
		}

		private void WriteBody(TextWriter writer, IRequestParameter body) {
			// TODO:
		}

		private string MakeResourceandQuery(IRestRequest request) {
			var resource = MakeResource(request);
			var sb = new StringBuilder(resource);
			if (request.HasQueryString()) {
				sb.Append("?");
				foreach (var pair in request.QueryStringPairs()) {
					sb.Append(pair.Key);
					sb.Append('=');
					sb.Append(WebUtility.UrlEncode(pair.Value.ToString()));
				}
			}

			return sb.ToString();
		}

		private string MakeResource(IRestRequest request) {
			var resource = request.Resource;
			foreach (var route in request.Routes()) {
				var key = new StringBuilder().Append('{').Append(route.Key).Append('}').ToString();
				resource = resource.Replace(key, route.Value.ToString());
			}

			return resource;
		}
	}
}