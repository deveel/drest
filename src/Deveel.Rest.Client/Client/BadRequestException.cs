using System;
using System.Net;

namespace Deveel.Web.Client {
	public sealed class BadRequestException : RestResponseException {
		public BadRequestException(string message, string reasonPhrase)
			: base(message, HttpStatusCode.BadRequest, reasonPhrase) {
		}

		public BadRequestException(string reasonPhrase)
			: base(HttpStatusCode.BadRequest, reasonPhrase) {
		}
	}
}