using System;

namespace Deveel.Web.Client {
	public interface IRequestAuthenticator {
		void AuthenticateRequest(RestClient client, RestRequest request);
	}
}