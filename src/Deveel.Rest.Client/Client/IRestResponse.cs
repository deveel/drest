using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRestResponse {
		IRestRequest Request { get; }

		HttpStatusCode StatusCode { get; }

		string ReasonPhrase { get; }

		IEnumerable<KeyValuePair<string, string>> Headers { get; }


		void AssertSuccessful();

		Task<object> GetBodyAsync(CancellationToken cancellationToken);
	}
}