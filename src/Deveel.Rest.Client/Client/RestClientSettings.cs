using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using Newtonsoft.Json;

namespace Deveel.Web.Client {
	public class RestClientSettings : IRestClientSettings {
		public RestClientSettings() {
			ContentFormat = ContentFormat.Json;

			DefaultHeaders = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			RequestHandlers = new List<IHttpRequestHandler>();
			ResponseHandlers = new List<IHttpResponseHandler>();

			Serializers = new List<IContentSerializer> {
				new JsonContentSerializer(),
				new XmlContentSerializer(),
				new KeyValueContentSerializer()
			};

			ClientVersion = Assembly.GetEntryAssembly().GetName().Version;
			ClientName = Assembly.GetEntryAssembly().GetName().Name;
		}

		public string BaseUri { get; set; }

		public ContentFormat ContentFormat { get; set; }

		public Version ClientVersion { get; set; }

		public string ClientName { get; set; }

		public IDictionary<string, object> DefaultHeaders { get; set; }

		IEnumerable<KeyValuePair<string, object>> IRestClientSettings.DefaultHeaders => DefaultHeaders;

		public ICollection<IHttpRequestHandler> RequestHandlers { get; set; }

		public ICollection<IHttpResponseHandler> ResponseHandlers { get; set; }

		public ICollection<IContentSerializer> Serializers { get; set; }

		IEnumerable<IContentSerializer> IRestClientSettings.Serializers => Serializers;
	}
}