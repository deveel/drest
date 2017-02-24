using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public interface IHttpRequestHandler {
		Task HandleRequestAsync(RestClient client, HttpRequestMessage request);
	}
}