using System;
using System.Net.Http;

namespace Deveel.Web.Client {
	public interface IRequestBuilder {
		IRequestBuilder Method(HttpMethod method);

		IRequestBuilder To(string resource);

		IRequestBuilder With(IRequestParameter parameter);

		IRequestBuilder With(HttpFile file);

		IRequestBuilder Authenticate(bool value = true);

		IRequestBuilder Returns(Type returnType);

		IRequestBuilder UseAuthenticator(IRequestAuthenticator authenticator);

		RestRequest Build();
	}
}