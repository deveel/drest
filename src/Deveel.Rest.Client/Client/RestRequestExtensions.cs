﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

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

		public static IDictionary<string, object> QueryStringPairs(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.QueryString);
		}

		public static bool HasQueryString(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.QueryString);
		}

		public static IDictionary<string, object> Routes(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.Route);
		}

		public static bool HasRoute(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Route);
		}

		public static IDictionary<string, object> Headers(this IRestRequest request) {
			return request.ParametersDictionary(RequestParameterType.Header);
		}

		public static bool HasHeaders(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Header);
		}

		public static bool HasBody(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.Body);
		}

		public static IRequestParameter Body(this IRestRequest request) {
			return request.Parameters?.FirstOrDefault(x => x.IsBody());
		}

		public static bool HasFiles(this IRestRequest request) {
			return request.HasParameters(RequestParameterType.File);
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

		private static HttpContent MakeFileMultipart(IEnumerable<IRequestParameter> files) {
			var content = new MultipartFormDataContent();

			foreach (var file in files) {
				content.Add(file.GetFileContent(true));
			}

			return content;
		}

		public static HttpRequestMessage AsHttpRequestMessage(this IRestRequest request, IRestClient client) {
			var uri = UriHelper.MakeUri(client.Settings.BaseUri, request);

			var httpRequest = new HttpRequestMessage(request.Method, uri);

			if (request.Method == HttpMethod.Post || 
				request.Method == HttpMethod.Put) {
				if (request.HasBody()) {
					httpRequest.Content = request.Body().GetHttpContent(client);
				} else if (request.HasFiles()) {
					var files = request.Files().ToList();
					if (files.Count > 1) {
						httpRequest.Content = MakeFileMultipart(request.Files());
					} else {
						httpRequest.Content = files[0].GetFileContent(false);
					}
				}
			}

			if (request.HasHeaders()) {
				foreach (var header in request.Headers()) {
					httpRequest.Headers.Add(header.Key, SafeValue(header.Value));
				}
			}

			return httpRequest;
		}

	}
}