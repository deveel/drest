using System;
using System.Net;

namespace Deveel.Web.Client {
	public sealed class NotFoundException : RestResponseException {
		public NotFoundException(string message, string reasonPhrase)
			: base(message, HttpStatusCode.NotFound, reasonPhrase) {
		}

		public NotFoundException(string reasonPhrase)
			: base(HttpStatusCode.NotFound, reasonPhrase) {
		}
	}
}