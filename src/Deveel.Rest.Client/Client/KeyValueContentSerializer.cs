using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Deveel.Web.Client {
	public class KeyValueContentSerializer : IContentSerializer {
		ContentFormat IContentSerializer.SupportedFormat => ContentFormat.KeyValue;

		string[] IContentSerializer.ContentTypes => new[] { "application/x-www-form-urlencoded" };

		public string Serialize(IRestClient client, object obj) {
			if (obj == null)
				return null;
			if (obj is string)
				throw new NotSupportedException();

			var values = obj.GetType()
				.GetTypeInfo()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null))
				.Select(x => $"{x.Key}={SafeValue(client, x.Value)}");

			return String.Join("&", values);
		}

		private static string SafeValue(IRestClient client, object obj) {
			if (obj == null)
				return "";

			var culture = client.Settings.DefaultCulture ?? CultureInfo.InvariantCulture;
			var s = Convert.ToString(obj, culture);
			return WebUtility.UrlEncode(s);
		}

		public object Deserialize(IRestClient client, Type type, string source) {
			if (String.IsNullOrEmpty(source))
				return null;

			var parts = source.Split('&')
				.Select(x => x.Split('='))
				.ToDictionary(part => part[0], part => part[1]);

			var obj = Activator.CreateInstance(type, true);
			var props = obj.GetType()
				.GetTypeInfo()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var prop in props) {
				var propType = prop.PropertyType;

				string invalue;
				if (!parts.TryGetValue(prop.Name, out invalue)) {
					prop.SetValue(obj, null, null);
				} else {
					var culture = client.Settings.DefaultCulture ?? CultureInfo.InvariantCulture;
					var converted = Convert.ChangeType(invalue, propType, culture);
					prop.SetValue(obj, converted);
				}
			}

			return obj;
		}
	}
}