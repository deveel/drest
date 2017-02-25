using System;

namespace Deveel.Web.Client {
	public interface IRequestAuthenticator {
		void AuthenticateRequest(IRestClient client, IRestRequest request);
	}
}