using System;

using Newtonsoft.Json;

namespace Deveel.Web.Client {
	public class JsonContentSerializer : IContentSerializer {
		public JsonContentSerializer() 
			: this(new JsonSerializerSettings()) {
		}

		ContentFormat IContentSerializer.SupportedFormat => ContentFormat.Json;

		public JsonContentSerializer(JsonSerializerSettings settings) {
			Settings = settings;
		}

		string[] IContentSerializer.ContentTypes => new[] {
			"application/json"
			// TODO: more?
		};

		public JsonSerializerSettings Settings { get; }

		public string Serialize(IRestClient client, object obj) {
			var settings = Settings;
			if (client.Settings.DefaultCulture != null)
				settings.Culture = client.Settings.DefaultCulture;

			return JsonConvert.SerializeObject(obj, settings);
		}

		public object Deserialize(IRestClient client, Type type, string source) {
			var settings = Settings;
			if (client.Settings.DefaultCulture != null)
				settings.Culture = client.Settings.DefaultCulture;

			return JsonConvert.DeserializeObject(source, type, settings);
		}
	}
}