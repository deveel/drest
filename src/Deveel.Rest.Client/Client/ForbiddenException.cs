using System;
using System.Net;

namespace Deveel.Web.Client {
	public class ForbiddenException : RestResponseException {
		public ForbiddenException(string message, string reasonPhrase)
			: base(message, HttpStatusCode.Forbidden, reasonPhrase) {
		}

		public ForbiddenException(string reasonPhrase)
			: base(HttpStatusCode.Forbidden, reasonPhrase) {
		}
	}
}