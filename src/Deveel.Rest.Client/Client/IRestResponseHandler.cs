using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IRestResponseHandler {
		Task HandleResponseAsync(IRestClient client, IRestResponse response);
	}
}