using System;
using System.Text;

namespace Deveel.Web.Client {
	public sealed class BasicRequestAuthenticator : IRequestAuthenticator {
		private readonly string token;

		public BasicRequestAuthenticator(string userName, string password) {
			UserName = userName;
			Password = password;

			token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
		}

		public string UserName { get; }

		public string Password { get; }

		void IRequestAuthenticator.AuthenticateRequest(IRestClient client, IRestRequest request) {
			if (!request.HasHeader("Authorization"))
				request.AddHeader("Authorization", $"Basic {token}");
		}
	}
}