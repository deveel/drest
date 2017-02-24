using System;
using System.Net;

namespace Deveel.Web.Client {
	public sealed class ConflictException : RestResponseException {
		public ConflictException(string message, string reasonPhrase)
			: base(message, HttpStatusCode.Conflict, reasonPhrase) {
		}

		public ConflictException(string reasonPhrase)
			: base(HttpStatusCode.Conflict, reasonPhrase) {
		}
	}
}