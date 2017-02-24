using System;
using System.Net;

namespace Deveel.Web.Client {
	public sealed class UnauthorizedException : RestResponseException {
		public UnauthorizedException(string message, string reasonPhrase)
			: base(message, HttpStatusCode.Unauthorized, reasonPhrase) {
		}

		public UnauthorizedException(string reasonPhrase)
			: base(HttpStatusCode.Unauthorized, reasonPhrase) {
		}
	}
}