using System;

namespace Deveel.Web.Client {
	public sealed class JwtRequestAuthenticator :IRequestAuthenticator {
		public JwtRequestAuthenticator(string token) {
			Token = token;
		}

		public string Token { get; }

		void IRequestAuthenticator.AuthenticateRequest(IRestClient client, IRestRequest request) {
			if (!request.HasHeader("Authorization"))
				request.AddHeader("Authorization", $"Bearer {Token}");
		}
	}
}