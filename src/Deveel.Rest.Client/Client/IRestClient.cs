using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRestClient {
		IRestClientSettings Settings { get; }


		void HandleFailResponse(IRestResponse response);

		Task<IRestResponse> RequestAsync(IRestRequest request, CancellationToken cancellationToken);
	}
}