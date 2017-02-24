using System;
using System.Collections.Generic;
using System.Linq;

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
	}
}