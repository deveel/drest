using System;
using System.Linq;

namespace Deveel.Web.Client {
	public static class ClientSettingsExtensions {
		public static bool Supports(this IRestClientSettings settings, ContentFormat format) {
			return settings.Serializers != null && settings.Serializers.Any(x => x.SupportedFormat == format);
		}

		public static bool SupportsXml(this IRestClientSettings settings) => settings.Supports(ContentFormat.Xml);

		public static bool SupportsJson(this IRestClientSettings settings) => settings.Supports(ContentFormat.Json);

		public static bool SupportsKeyValue(this IRestClientSettings settings) => settings.Supports(ContentFormat.KeyValue);

		public static IContentSerializer Serializer(this IRestClientSettings settings, ContentFormat format) {
			return settings.Serializers != null ? settings.Serializers.FirstOrDefault(x => x.SupportedFormat == format) : null;
		}

		public static IContentSerializer JsonSerializer(this IRestClientSettings settings) {
			return settings.Serializer(ContentFormat.Json);
		}

		public static IContentSerializer XmlSerializer(this IRestClientSettings settings) {
			return settings.Serializer(ContentFormat.Xml);
		}

		public static IContentSerializer KeyValueSerializer(this IRestClientSettings settings) {
			return settings.Serializer(ContentFormat.KeyValue);
		}
	}
}