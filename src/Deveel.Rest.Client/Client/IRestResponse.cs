using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRestResponse {
		IRestClient Client { get; }

		IRestRequest Request { get; }

		HttpStatusCode StatusCode { get; }

		string ReasonPhrase { get; }

		IEnumerable<KeyValuePair<string, string>> Headers { get; }


		Task<object> GetBodyAsync(CancellationToken cancellationToken);
	}
}