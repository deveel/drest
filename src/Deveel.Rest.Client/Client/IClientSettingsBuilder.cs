using System;
using System.Net.Http;

namespace Deveel.Web.Client {
	public interface IClientSettingsBuilder {
		IClientSettingsBuilder BaseUri(Uri baseUri);

		IClientSettingsBuilder UseSerializer(IContentSerializer serializer);

		IClientSettingsBuilder UseSerializer(Type serializerType);

		IClientSettingsBuilder OnRequest(IRequestHandler handler);

		IClientSettingsBuilder OnRequest(Type handlerType);

		IClientSettingsBuilder OnResponse(IRestResponseHandler handler);

		IClientSettingsBuilder OnResponse(Type handlerType);

		IClientSettingsBuilder UseMessageHandler(HttpMessageHandler handler);

		IClientSettingsBuilder DefaultToFormat(ContentFormat format);

		IClientSettingsBuilder WithtHeader(string key, object value);

		IClientSettingsBuilder UseAuthenticator(IRequestAuthenticator authenticator);

		IClientSettingsBuilder AuthenticateRequests(bool value = true);

		IRestClientSettings Build(IBuildContext context);
	}
}