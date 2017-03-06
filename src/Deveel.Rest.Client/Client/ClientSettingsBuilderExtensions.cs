using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Newtonsoft.Json;

namespace Deveel.Web.Client {
	public static class ClientSettingsBuilderExtensions {
		public static IClientSettingsBuilder BaseUri(this IClientSettingsBuilder builder, string baseUri) {
			if (String.IsNullOrEmpty(baseUri))
				throw new ArgumentNullException(nameof(baseUri));

			Uri uri;
			if (!Uri.TryCreate(baseUri, UriKind.Absolute, out uri))
				throw new ArgumentException($"The string {baseUri} is not well-formed");

			return builder.BaseUri(uri);
		}

		public static IClientSettingsBuilder UseXmlSerializer(this IClientSettingsBuilder builder,
			XmlReaderSettings readerSettings, XmlWriterSettings writerSettings) {
			return builder.UseSerializer(new XmlContentSerializer(readerSettings, writerSettings));
		}

		public static IClientSettingsBuilder UseXmlSerializer(this IClientSettingsBuilder builder,
			XmlReaderSettings readerSettings) {
			return builder.UseXmlSerializer(readerSettings, new XmlWriterSettings());
		}

		public static IClientSettingsBuilder UseXmlSerializer(this IClientSettingsBuilder builder,
			XmlWriterSettings writerSettings) {
			return builder.UseXmlSerializer(new XmlReaderSettings(), writerSettings);
		}

		public static IClientSettingsBuilder UseXmlSerializer(this IClientSettingsBuilder builder) {
			return builder.UseXmlSerializer(new XmlReaderSettings(), new XmlWriterSettings());
		}

		public static IClientSettingsBuilder UseJsonSerializer(this IClientSettingsBuilder builder,
			JsonSerializerSettings settings) {
			return builder.UseSerializer(new JsonContentSerializer(settings));
		}

		public static IClientSettingsBuilder UseJsonSerializer(this IClientSettingsBuilder builder) {
			return builder.UseJsonSerializer(new JsonSerializerSettings());
		}

		public static IClientSettingsBuilder UseKeyValueSerializer(this IClientSettingsBuilder builder) {
			return builder.UseSerializer(new KeyValueContentSerializer());
		}

		public static IClientSettingsBuilder UseSerializer<T>(this IClientSettingsBuilder builder) 
			where T : class, IContentSerializer {
			return builder.UseSerializer(typeof(T));
		}

		public static IClientSettingsBuilder UseDefaultSerializers(this IClientSettingsBuilder builder) {
			return builder
				.UseJsonSerializer()
				.UseXmlSerializer()
				.UseKeyValueSerializer();
		}

		public static IClientSettingsBuilder WithHeaders(this IClientSettingsBuilder builder,
			IEnumerable<KeyValuePair<string, object>> headers) {
			foreach (var header in headers) {
				builder = builder.WithtHeader(header.Key, header.Value);
			}

			return builder;
		}

		public static IClientSettingsBuilder OnRequest<T>(this IClientSettingsBuilder builder)
			where T : class, IRequestHandler {
			return builder.OnRequest(typeof(T));
		}

		public static IClientSettingsBuilder OnRequest(this IClientSettingsBuilder builder, Func<IRestClient, IRestRequest, Task> handler) {
			return builder.OnRequest(new DelegatedRequestHandler(handler));
		}

		public static IClientSettingsBuilder OnRequest(this IClientSettingsBuilder builder,
			Func<IRestRequest, Task> handler) {
			return builder.OnRequest((client, request) => handler(request));
		}

		public static IClientSettingsBuilder OnRequest(this IClientSettingsBuilder builder, 
			Action<IRestClient, IRestRequest> handler) {
			return builder.OnRequest((client, request) => {
				handler(client, request);
				return Task.CompletedTask;
			});
		}

		public static IClientSettingsBuilder OnRequest(this IClientSettingsBuilder builder, Action<IRestRequest> handler) {
			return builder.OnRequest((client, request) => handler(request));
		}

		public static IClientSettingsBuilder OnResponse<T>(this IClientSettingsBuilder builder)
			where T : class, IRestResponseHandler {
			return builder.OnResponse(typeof(T));
		}

		public static IClientSettingsBuilder OnResponse(this IClientSettingsBuilder builder,
			Func<IRestClient, IRestResponse, Task> handler) {
			return builder.OnResponse(new DelegatedRestResponseHandler(handler));
		}

		public static IClientSettingsBuilder OnResponse(this IClientSettingsBuilder builder, Func<IRestResponse, Task> handler) {
			return builder.OnResponse((client, request) => handler(request));
		}

		public static IClientSettingsBuilder OnResponse(this IClientSettingsBuilder builder, Action<IRestClient, IRestResponse> handler) {
			return builder.OnResponse((client, response) => {
				handler(client, response);
				return Task.CompletedTask;
			});
		}

		public static IClientSettingsBuilder OnResponse(this IClientSettingsBuilder builder, Action<IRestResponse> handler) {
			return builder.OnResponse((client, response) => handler(response));
		}

		public static IClientSettingsBuilder DefaultToJson(this IClientSettingsBuilder builder) {
			return builder.DefaultToFormat(ContentFormat.Json);
		}

		public static IClientSettingsBuilder DefaultToXml(this IClientSettingsBuilder builder) {
			return builder.DefaultToFormat(ContentFormat.Xml);
		}

		public static IClientSettingsBuilder DefaultToKeyValue(this IClientSettingsBuilder builder) {
			return builder.DefaultToFormat(ContentFormat.KeyValue);
		}

		public static IClientSettingsBuilder UseBasicAuthentication(this IClientSettingsBuilder builder, string userName, string password) {
			return builder.UseAuthenticator(new BasicRequestAuthenticator(userName, password));
		}

		public static IClientSettingsBuilder UseJwtAuthentication(this IClientSettingsBuilder builder, string token) {
			return builder.UseAuthenticator(new JwtRequestAuthenticator(token));
		}

		public static IClientSettingsBuilder UseEncoding(this IClientSettingsBuilder builder, string encoding) {
			return builder.UseEncoding(Encoding.GetEncoding(encoding));
		}

		public static IClientSettingsBuilder UseUTF8(this IClientSettingsBuilder builder) {
			return builder.UseEncoding(Encoding.UTF8);
		}

		public static IClientSettingsBuilder UseASCII(this IClientSettingsBuilder builder) {
			return builder.UseEncoding(Encoding.ASCII);
		}

		public static IClientSettingsBuilder UseInvariantCulture(this IClientSettingsBuilder builder) {
			return builder.UseCulture(CultureInfo.InvariantCulture);
		}

		public static IClientSettingsBuilder UseDefaultCulture(this IClientSettingsBuilder builder) {
			return builder.UseInvariantCulture();
		}

		public static IClientSettingsBuilder UseCurrentCulture(this IClientSettingsBuilder builder) {
			return builder.UseCulture(CultureInfo.CurrentCulture);
		}

		public static IClientSettingsBuilder UseCulture(this IClientSettingsBuilder builder, string cultureName) {
			return builder.UseCulture(new CultureInfo(cultureName));
		}

		public static IRestClientSettings Build(this IClientSettingsBuilder builder) {
			return builder.Build(new DefaultBuildContext());
		}

		#region DelegatedRequestHandler

		class DelegatedRequestHandler : IRequestHandler {
			private readonly Func<IRestClient, IRestRequest, Task> handler;

			public DelegatedRequestHandler(Func<IRestClient, IRestRequest, Task> handler) {
				this.handler = handler;
			}

			public Task HandleRequestAsync(IRestClient client, IRestRequest request) {
				return handler(client, request);
			}
		}

		#endregion

		#region DelegatedResponseHandler

		class DelegatedRestResponseHandler : IRestResponseHandler {
			private readonly Func<IRestClient, IRestResponse, Task> handler;

			public DelegatedRestResponseHandler(Func<IRestClient, IRestResponse, Task> handler) {
				this.handler = handler;
			}

			public Task HandleResponseAsync(IRestClient client, IRestResponse response) {
				return handler(client, response);
			}
		}

		#endregion
	}
}