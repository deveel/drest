using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IHttpResponseHandler {
		Task HandleResponseAsync(RestClient client, HttpResponseMessage response);
	}
}