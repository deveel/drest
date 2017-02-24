using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Deveel.Web.Client {
	class ClientSettingsBuilder : IClientSettingsBuilder {
		private Uri serviceBaseUri;
		private List<Handler> serializers;
		private List<Handler> requestHandlers;
		private List<Handler> responseHandlers;
		private HttpMessageHandler messageHandler;
		private Dictionary<string, object> defaultHeaders;
		private ContentFormat defaultFormat;
		private IRequestAuthenticator requestAuthenticator;

		public IClientSettingsBuilder BaseUri(Uri baseUri) {
			if (baseUri == null)
				throw new ArgumentNullException(nameof(baseUri));

			if (baseUri.Scheme != "http" && baseUri.Scheme != "https")
				throw new ArgumentException($"Invalid URI scheme.");

			serviceBaseUri = baseUri;
			return this;
		}

		public IClientSettingsBuilder UseSerializer(IContentSerializer serializer) {
			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer));

			if (serializers == null)
				serializers = new List<Handler>();

			serializers.Add(new Handler(serializer));
			return this;
		}

		public IClientSettingsBuilder UseSerializer(Type serializerType) {
			if (serializerType == null)
				throw new ArgumentNullException(nameof(serializerType));
			if (!typeof(IContentSerializer).GetTypeInfo().IsAssignableFrom(serializerType))
				throw new ArgumentException($"The given type {serializerType} is not assignable from {typeof(IContentSerializer)}");

			if (serializers == null)
				serializers = new List<Handler>();

			serializers.Add(new Handler(serializerType));
			return this;

		}

		public IClientSettingsBuilder OnRequest(IRequestHandler handler) {
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			if (requestHandlers == null)
				requestHandlers = new List<Handler>();

			requestHandlers.Add(new Handler(handler));
			return this;
		}

		public IClientSettingsBuilder OnRequest(Type handlerType) {
			if (handlerType == null)
				throw new ArgumentNullException(nameof(handlerType));
			if (!typeof(IRequestHandler).GetTypeInfo().IsAssignableFrom(handlerType))
				throw new ArgumentException($"The given type {handlerType} is not assignable from {typeof(IRequestHandler)}");

			if (requestHandlers == null)
				requestHandlers = new List<Handler>();

			requestHandlers.Add(new Handler(handlerType));
			return this;

		}

		public IClientSettingsBuilder OnResponse(Type handlerType) {
			if (handlerType == null)
				throw new ArgumentNullException(nameof(handlerType));
			if (!typeof(IRestResponseHandler).GetTypeInfo().IsAssignableFrom(handlerType))
				throw new ArgumentException($"The given type {handlerType} is not assignable from {typeof(IRestResponseHandler)}");

			if (responseHandlers == null)
				responseHandlers = new List<Handler>();

			responseHandlers.Add(new Handler(handlerType));
			return this;
		}


		public IClientSettingsBuilder OnResponse(IRestResponseHandler handler) {
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			if (responseHandlers == null)
				responseHandlers = new List<Handler>();

			responseHandlers.Add(new Handler(handler));
			return this;
		}

		public IClientSettingsBuilder UseAuthenticator(IRequestAuthenticator authenticator) {
			if (authenticator == null)
				throw new ArgumentNullException(nameof(authenticator));

			requestAuthenticator = authenticator;
			return this;
		}

		public IClientSettingsBuilder UseMessageHandler(HttpMessageHandler handler) {
			messageHandler = handler;
			return this;
		}

		public IClientSettingsBuilder DefaultToFormat(ContentFormat format) {
			if (format == Client.ContentFormat.Default)
				throw new ArgumentException("Invalid default format");

			defaultFormat = format;
			return this;
		}

		public IClientSettingsBuilder WithtHeader(string key, object value) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			if (defaultHeaders == null)
				defaultHeaders = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			defaultHeaders[key] = value;
			return this;
		}

		public IRestClientSettings Build(IBuildContext context) {
			if (serviceBaseUri == null)
				throw new InvalidOperationException("No base URI address specified.");

			var settings = new RestClientSettings {
				BaseUri = serviceBaseUri,
				MessageHandler = messageHandler,
				Authenticator = requestAuthenticator,
				DefaultFormat = defaultFormat == ContentFormat.Default ? ContentFormat.Json : defaultFormat
			};

			AddRequestHandlers(settings, context);
			AddResponseHandlers(settings, context);
			AddSerializers(settings, context);

			return settings;
		}

		private void AddSerializers(RestClientSettings settings, IBuildContext context) {
			if (serializers != null) {
				foreach (var handler in serializers) {
					var requestHandler = handler.Instance as IContentSerializer;
					if (handler.Type != null)
						requestHandler = context.Resolve(handler.Type) as IContentSerializer;

					settings.Serializers.Add(requestHandler);
				}
			}
		}

		private void AddRequestHandlers(RestClientSettings settings, IBuildContext context) {
			if (requestHandlers != null) {
				foreach (var handler in requestHandlers) {
					var requestHandler = handler.Instance as IRequestHandler;
					if (handler.Type != null)
						requestHandler = context.Resolve(handler.Type) as IRequestHandler;
					
					settings.RequestHandlers.Add(requestHandler);
				}
			}
		}

		private void AddResponseHandlers(RestClientSettings settings, IBuildContext context) {
			if (requestHandlers != null) {
				foreach (var handler in responseHandlers) {
					var requestHandler = handler.Instance as IRestResponseHandler;
					if (handler.Type != null)
						requestHandler = context.Resolve(handler.Type) as IRestResponseHandler;

					settings.ResponseHandlers.Add(requestHandler);
				}
			}
		}

		#region Handler

		class Handler {
			public Handler(Type type) {
				Type = type;
			}

			public Handler(object instance) {
				Instance = instance;
			}

			public Type Type { get; }

			public object Instance { get; }
		}

		#endregion
	}
}