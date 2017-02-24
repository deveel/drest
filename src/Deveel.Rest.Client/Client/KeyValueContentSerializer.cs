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

		public string Serialize(object obj) {
			if (obj == null)
				return null;
			if (obj is string)
				throw new NotSupportedException();

			var values = obj.GetType()
				.GetTypeInfo()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null))
				.Select(x => $"{x.Key}={SafeValue(x)}");

			return String.Join("&", values);
		}

		private static string SafeValue(object obj) {
			if (obj == null)
				return "";

			var s = Convert.ToString(obj, CultureInfo.InvariantCulture);
			return WebUtility.UrlEncode(s);
		}

		public object Deserialize(Type type, string source) {
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
					var converted = Convert.ChangeType(invalue, propType, CultureInfo.InvariantCulture);
					prop.SetValue(obj, converted);
				}
			}

			return obj;
		}
	}
}