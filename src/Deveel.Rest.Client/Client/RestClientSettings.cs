using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Deveel.Web.Client {
	public class RestClientSettings : IRestClientSettings {
		public RestClientSettings() {
			DefaultFormat = ContentFormat.Json;

			DefaultHeaders = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			RequestHandlers = new List<IRequestHandler>();
			ResponseHandlers = new List<IRestResponseHandler>();
			Serializers = new List<IContentSerializer> {
				new JsonContentSerializer(),
				new KeyValueContentSerializer(),
				new XmlContentSerializer()
			};

			ContentTypeProvider = new DefaultContentTypeProvider();
			ContentEncoding = Encoding.UTF8;
			DefaultCulture = CultureInfo.InvariantCulture;
		}

		public Uri BaseUri { get; set; }

		public ContentFormat DefaultFormat { get; set; }

		public IDictionary<string, object> DefaultHeaders { get; set; }

		IEnumerable<KeyValuePair<string, object>> IRestClientSettings.DefaultHeaders => DefaultHeaders;

		public ICollection<IRequestHandler> RequestHandlers { get; set; }

		IEnumerable<IRequestHandler> IRestClientSettings.RequestHandlers => RequestHandlers;

		public ICollection<IRestResponseHandler> ResponseHandlers { get; set; }

		IEnumerable<IRestResponseHandler> IRestClientSettings.ResponseHandlers => ResponseHandlers;

		public ICollection<IContentSerializer> Serializers { get; set; }

		IEnumerable<IContentSerializer> IRestClientSettings.Serializers => Serializers;

		public HttpMessageHandler MessageHandler { get; set; }

		public IRequestAuthenticator Authenticator { get; set; }

		public bool AuthenticateRequests { get; set; }

		public IContentTypeProvider ContentTypeProvider { get; set; }

		public Encoding ContentEncoding { get; set; }

		public CultureInfo DefaultCulture { get; set; }
	}
}