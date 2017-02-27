using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Deveel.Web.Client {
	public static class RestRequestExtensions {
		private static IDictionary<string, object> ParametersDictionary(this IRestRequest request,
			RequestParameterType parameterType) {
			return request.Parameters == null
				? new Dictionary<string, object>()
				: request.Parameters.Where(x => x.Type == parameterType)
					.ToDictionary(x => x.Name, y => y.Value, StringComparer.OrdinalIgnoreCase);
		}

		private static bool HasParameters(this IRestRequest request, RequestParameterType parameterType) {
			return request.Parameters != null && request.Parameters.Any(x => x.Type == parameterType);
		}

		public static bool HasParameter(this IRestRequest request, RequestParameterType type, string key) {
			return request.ParametersDictionary(type).ContainsKey(key);
		}

		public static void AddParameter(this IRestRequest request, IRequestParameter parameter) {
			if (parameter == null)
				throw new ArgumentNullException(nameof(parameter));

			var parameters = request.Parameters;
			if (parameters is ICollection<IRequestParameter>) {
				var list = (ICollection<IRequestParameter>) parameters;
				if (list.IsReadOnly)
					throw new ArgumentException("The parameter list is read-only.");

				list.Add(parameter);
			} else {
				throw new NotSupportedException("Cannot add the parameter to the request.");
			}
		}

		public static void AddParameter(this IRestRequest request, RequestParameterType type, string key, object value) {
			request.AddParameter(new SimpleRequestParameter(type, key, value));
		}

		public static IDictionary<string, object> QueryStringPairs(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.QueryString);
		}

		public static bool HasQueryString(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.QueryString);
		}

		public static bool HasQueryStringPair(this IRestRequest request, string key) {
			return request.HasParameter(RequestParameterType.QueryString, key);
		}

		public static void AddQueryStringPair(this IRestRequest request, string key, object value) {
			request.AddParameter(RequestParameterType.QueryString, key, value);
		}

		public static IDictionary<string, object> Routes(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.Route);
		}

		public static bool HasRoutes(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Route);
		}

		public static bool HasRoute(this IRestRequest request, string key) {
			return request.HasParameter(RequestParameterType.Route, key);
		}

		public static void AddRoute(this IRestRequest request, string key, object value) {
			request.AddParameter(RequestParameterType.Route, key, value);
		}

		public static IDictionary<string, object> Headers(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.Header);
		}

		public static bool HasHeaders(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Header);
		}

		public static bool HasHeader(this IRestRequest request, string key) {
			return request.HasParameter(RequestParameterType.Header, key);
		}

		public static void AddHeader(this IRestRequest request, string key, object value) {
			request.AddParameter(RequestParameterType.Header, key, value);
		}

		public static bool HasBody(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Body);
		}

		public static void AddBody(this IRestRequest request, object value) {
			request.AddParameter(RequestParameterType.Body, "body", value);
		}

		public static IRequestParameter Body(this IRestRequest request) {
			return request.Parameters?.FirstOrDefault(x => x.IsBody());
		}

		public static bool HasFiles(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.File);
		}

		public static void AddFile(this IRestRequest request, string name, string fileName, string contentType, Stream content) {
			request.AddParameter(new RequestFile(name, fileName, contentType, content));
		}

		public static bool HasFile(this IRestRequest request, string key) {
			return request.HasParameter(RequestParameterType.File, key);
		}

		public static IEnumerable<IRequestParameter> Files(this IRestRequest request) {
			return request.Parameters.Where(x => x.IsFile());
		}

		public static string ToString(this IRestRequest request, IRequestFormatter formatter) {
			return formatter.Format(request);
		}

		private static string SafeValue(object value) {
			return value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		private static HttpContent MakeFileMultipart(IRestClient client, IEnumerable<IRequestParameter> files) {
			var content = new MultipartFormDataContent();

			foreach (var file in files) {
				var fileName = file.FileName();
				content.Add(file.GetFileContent(client, true), file.Name, fileName);
			}

			return content;
		}

		internal static HttpRequestMessage AsHttpRequestMessage(this IRestRequest request, IRestClient client) {
			var uri = UriHelper.MakeUri(client.Settings.BaseUri, request);

			var httpRequest = new HttpRequestMessage(request.Method, uri);

			if (request.Method == HttpMethod.Post || 
				request.Method == HttpMethod.Put) {
				HttpContent content = null;

				if (request.HasBody()) {
					content = request.Body().GetHttpContent(client);
				} else if (request.HasFiles()) {
					var files = request.Files().ToList();
					if (files.Count > 1) {
						content = MakeFileMultipart(client, request.Files());
					} else {
						content = files[0].GetFileContent(client, false);
					}
				}

				httpRequest.Content = content;
			}

			if (request.HasHeaders()) {
				foreach (var header in request.Headers()) {
					httpRequest.Headers.Add(header.Key, SafeValue(header.Value));
				}
			}

			if (request.ReturnedType != null) {
				var contentFormat = request.ReturnedFormat;
				if (contentFormat == ContentFormat.Default)
					contentFormat = client.Settings.DefaultFormat;

				var serializer = client.Settings.Serializer(contentFormat);
				if (serializer == null)
					throw new InvalidOperationException($"No serializer was configured to handle the format {contentFormat} required.");

				var contentTypes = serializer.ContentTypes;

				foreach (var contentType in contentTypes) {
					httpRequest.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(contentType));
				}
			}

			return httpRequest;
		}

		public static string ToHttpString(this IRestRequest request, IRestClient client) {
			return request.ToString(new HttpRequestFormatter(client));
		}
	}
}