using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Deveel.Web.Client {
	public static class RequestBodyExtensions {
		public static bool IsMultiparted(this IRequestBody body) {
			return body is IMultipartBody &&
			       ((IMultipartBody) body).Parts != null &&
			       ((IMultipartBody) body).Parts.Any();
		}

		public static bool IsNamed(this IRequestBody body) {
			return !String.IsNullOrEmpty(body.Name);
		}

		public static IEnumerable<KeyValuePair<string, IBodyPart>> Parts(this IRequestBody body) {
			return body is IMultipartBody ? ((IMultipartBody) body).Parts : new KeyValuePair<string, IBodyPart>[0];
		}
	}
}