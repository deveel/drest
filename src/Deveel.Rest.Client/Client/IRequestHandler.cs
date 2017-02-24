using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRequestHandler {
		Task HandleRequestAsync(IRestClient client, IRestRequest request);
	}
}