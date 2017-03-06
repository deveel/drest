using System;
using System.Reflection;

namespace Deveel.Web.Client {
	public static class RequestReturnExtensions {
		public static bool IsFile(this IRequestReturn requestReturn) {
			return requestReturn != null && 
				requestReturn.ReturnType != null && 
				typeof(IRequestFile).GetTypeInfo().IsAssignableFrom(requestReturn.ReturnType);
		}

		public static bool IsVoid(this IRequestReturn requestReturn) {
			return requestReturn == null ||
			       requestReturn.ReturnType == null ||
			       typeof(void).GetTypeInfo().IsAssignableFrom(requestReturn.ReturnType);
		}

		public static ContentFormat ContentFormat(this IRequestReturn requestReturn) {
			var contentType = !requestReturn.IsFile() && !requestReturn.IsFile() ? requestReturn.ContentType : null;
			if (String.IsNullOrEmpty(contentType))
				return Client.ContentFormat.Default;

			switch (contentType.ToLowerInvariant()) {
				case "application/json":
				case "text/json":
					return Client.ContentFormat.Json;
				case "text/xml":
				case "application/xml":
					return Client.ContentFormat.Xml;
				case "application/www-form-urlencoded":
					return Client.ContentFormat.KeyValue;
				default:
					return Client.ContentFormat.Default;
			}
		}
	}
}