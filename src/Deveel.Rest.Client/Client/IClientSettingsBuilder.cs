using System;
using System.Globalization;
using System.Net.Http;
using System.Text;

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

		IClientSettingsBuilder UseContentTypeProvider(IContentTypeProvider provider);

		IClientSettingsBuilder UseEncoding(Encoding encoding);

		IClientSettingsBuilder UseCulture(CultureInfo culture);

		IRestClientSettings Build(IBuildContext context);
	}
}