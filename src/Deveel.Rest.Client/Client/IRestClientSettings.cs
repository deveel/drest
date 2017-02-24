using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Deveel.Web.Client {
	public interface IRestClientSettings {
		IEnumerable<IContentSerializer> Serializers { get; }

		ContentFormat DefaultFormat { get; }

		IEnumerable<KeyValuePair<string, object>> DefaultHeaders { get; }

		HttpMessageHandler MessageHandler { get; set; }

		IEnumerable<IRequestHandler> RequestHandlers { get; }

		IEnumerable<IRestResponseHandler> ResponseHandlers { get; }

		IRequestAuthenticator Authenticator { get; }

		Uri BaseUri { get; }
	}
}