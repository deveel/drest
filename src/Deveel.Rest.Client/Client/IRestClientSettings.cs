using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Deveel.Web.Client {
	public interface IRestClientSettings {
		IEnumerable<IContentSerializer> Serializers { get; }

		ContentFormat DefaultFormat { get; set; }

		IEnumerable<KeyValuePair<string, object>> DefaultHeaders { get; }

		HttpMessageHandler MessageHandler { get; set; }

		IEnumerable<IRequestHandler> RequestHandlers { get; }

		IEnumerable<IRestResponseHandler> ResponseHandlers { get; }

		IRequestAuthenticator Authenticator { get; set; }

		IContentTypeProvider ContentTypeProvider { get; set; }

		Encoding ContentEncoding { get; set; }

		CultureInfo DefaultCulture { get; set; }

		bool AuthenticateRequests { get; set; }

		Uri BaseUri { get; set; }
	}
}