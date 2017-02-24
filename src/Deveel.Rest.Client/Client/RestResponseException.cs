using System;
using System.Net;

namespace Deveel.Web.Client {
	public class RestResponseException : Exception {
		public RestResponseException(HttpStatusCode statusCode, string reasonPhrase)
			: this($"{(int)statusCode} - {reasonPhrase}", statusCode, reasonPhrase) {
		}

		public RestResponseException(string message, HttpStatusCode statusCode, string reasonPhrase)
			: base(message) {
			StatusCode = statusCode;
			ReasonPhrase = reasonPhrase;
		}

		public HttpStatusCode StatusCode { get; }

		public string ReasonPhrase { get; }
	}
}