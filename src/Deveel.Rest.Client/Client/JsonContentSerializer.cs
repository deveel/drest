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

		public string Serialize(object obj) {
			return JsonConvert.SerializeObject(obj, Settings);
		}

		public object Deserialize(Type type, string source) {
			return JsonConvert.DeserializeObject(source, type, Settings);
		}
	}
}