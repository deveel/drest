using System;

namespace Deveel.Web.Client {
	public class AuthenticateException : Exception {
		public AuthenticateException() {
		}

		public AuthenticateException(string message)
			: base(message) {
		}
	}
}