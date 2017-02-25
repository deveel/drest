using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Deveel.Web.Client {
	public interface IRestRequest {
		HttpMethod Method { get; }

		string Resource { get; }

		IEnumerable<IRequestParameter> Parameters { get; }

		bool? Authenticate { get; }

		Type ReturnedType { get; }

		IRequestAuthenticator Authenticator { get; }
	}
}