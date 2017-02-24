using System;

namespace Deveel.Web.Client {
	public sealed class JwtRequestAuthenticator :IRequestAuthenticator {
		public JwtRequestAuthenticator(string token) {
			Token = token;
		}

		public string Token { get; }

		void IRequestAuthenticator.AuthenticateRequest(RestClient client, RestRequest request) {
			if (!request.Headers().ContainsKey("Authorization"))
				request.Parameters.Add(new SimpleRequestParameter(RequestParameterType.Header, "Authorization", $"Bearer {Token}"));
		}
	}
}