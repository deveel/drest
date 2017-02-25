using System;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Deveel.Web.Client {
	public static class RequestParameterExtensions {
		public static bool IsRoute(this IRequestParameter parameter) {
			return parameter.Type == RequestParameterType.Route;
		}

		public static bool IsQueryString(this IRequestParameter parameter) {
			return parameter.Type == RequestParameterType.QueryString;
		}

		public static bool IsHeader(this IRequestParameter parameter) {
			return parameter.Type == RequestParameterType.Header;
		}

		public static bool IsBody(this IRequestParameter parameter) {
			return parameter.Type == RequestParameterType.Body;
		}

		public static bool IsFile(this IRequestParameter parameter) {
			return parameter.Type == RequestParameterType.File;
		}

		public static string FileName(this IRequestParameter parameter) {
			if (parameter is IRequestFile)
				return ((IRequestFile) parameter).FileName;

			return $"{parameter.Name}.file";
		}

		internal static HttpContent GetHttpContent(this IRequestParameter parameter, IRestClient client) {
			return RequestBody.CreateContent(parameter, client);
		}

		internal static HttpContent GetFileContent(this IRequestParameter parameter, bool inMultipart) {
			return RequestFile.CreateFileContent(parameter, inMultipart);
		}

		public static bool IsSimpleValue(this IRequestParameter parameter) {
			return parameter.Value is string ||
				   parameter.Value is int ||
				   parameter.Value is short ||
				   parameter.Value is long ||
				   parameter.Value is double ||
				   parameter.Value is float ||
				   parameter.Value is decimal ||
				   parameter.Value is DateTime ||
				   parameter.Value is DateTimeOffset ||
				   parameter.Value is TimeSpan ||
				   parameter.Value is Guid;
		}

		internal static StringContent ValueAsString(this IRequestParameter parameter, IRestClient client) {
			// TODO: get this from the client's settings
			var s = parameter.Value == null ? null : Convert.ToString(parameter.Value, CultureInfo.InvariantCulture);
			return new StringContent(s, Encoding.UTF8, "text/plain");
		}
	}
}