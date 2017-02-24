using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Deveel.Web.Client {
	static class UriHelper {
		public static Uri MakeUri(Uri baseUri, IRestRequest request) {
			var resource = PrepareResource(request);
			var query = request.QueryStringPairs();

			var builder = new UriBuilder(baseUri);
			builder.Path = MakePath(builder.Path, resource);

			if (query != null && query.Count > 0) {
				builder.Query = MakeQueryString(query);
			}

			return builder.Uri;
		}

		private static string PrepareResource(IRestRequest request) {
			var resource = request.Resource;
			foreach (var pair in request.Routes()) {
				var key = new StringBuilder().Append("{").Append(pair.Key).Append("}").ToString();
				resource = resource.Replace(key, SafeValue(pair.Value));
			}

			return resource;
		}

		private static string SafeValue(object value) {
			return value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		private static string MakeQueryString(IDictionary<string, object> query) {
			return WebUtility.UrlEncode(String.Join("&", query.Select(x => $"{x.Key}={x.Value}")));
		}

		private static string MakePath(string path, string resource) {
			var sb = new StringBuilder();

			if (!String.IsNullOrEmpty(path)) {
				if (path[0] != '/')
					sb.Append('/');

				sb.Append(path);

				if (path[path.Length - 1] != '/')
					sb.Append('/');
			}

			if (String.IsNullOrEmpty(path) &&
			    resource[0] != '/')
				sb.Append('/');

			sb.Append(resource);

			return sb.ToString();
		}
	}
}