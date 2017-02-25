using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IHttpClient {
		Uri BaseAddress { get; set; }


		void AddDefaultHeader(string key, object value);

		Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken);
	}
}