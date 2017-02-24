using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRestClient {
		IRestClientSettings Settings { get; }

		Task<IRestResponse> RequestAsync(IRestRequest request, CancellationToken cancellationToken);
	}
}