using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Deveel.Web.Client {
	public sealed class RequestBodyParameter : IRequestParameter, IMultipartBodyParameter, IBodyPart {
		private IDictionary<string, IBodyPart> parts;

		public RequestBodyParameter(string name, object value) 
			: this(name, value, ContentFormat.Default) {
		}

		public RequestBodyParameter() 
			: this(null) {
		}

		public RequestBodyParameter(object value) 
			: this(value, ContentFormat.Default) {
		}

		public RequestBodyParameter(object value, ContentFormat format) 
			: this(null, value, format) {
		}

		public RequestBodyParameter(string name, object value, ContentFormat format) {
			Name = name;
			Value = value;
			Format = format;
		}

		public string Name { get; }

		public bool IsNamed => !String.IsNullOrEmpty(Name);

		public bool HasParts => parts != null && parts.Count > 0;

		public IEnumerable<KeyValuePair<string, object>> Parts =>
			parts == null
				? new KeyValuePair<string, object>[0]
				: parts.ToDictionary(x => x.Key, y => (object) y.Value).AsEnumerable();

		public ContentFormat Format { get; set; }

		RequestParameterType IRequestParameter.ParameterType {
			get { return RequestParameterType.Body; }
		}

		string IRequestParameter.ParameterName => Name;

		object IRequestParameter.ParameterValue => Value;

		public object Value { get; }

		private void AddPart(IBodyPart part) {
			if (String.IsNullOrEmpty(part.Name))
				throw new ArgumentException("A part of a multi-parted body must be named");

			if (parts == null)
				parts = new Dictionary<string, IBodyPart>();

			if (parts.ContainsKey(part.Name))
				throw new ArgumentException($"A part named {part.Name} is already in the body");

			parts.Add(part.Name, part);
		}

		public void AddPart(RequestBodyParameter part) {
			if (part == null)
				throw new ArgumentNullException(nameof(part));

			if (!part.IsNamed)
				throw new ArgumentException("A part must be named");
			if (part.HasParts)
				throw new ArgumentException("Cannot add a multi-parted body to a multi-part");

			if (part.Format != ContentFormat.Default &&
				part.Format != ContentFormat.KeyValue)
				throw new ArgumentException("A body part can only contain ");

			AddPart((IBodyPart)part);
		}

		public void AddFile(HttpFile file) {
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			AddPart(file);
		}

		internal HttpContent CreateContent(IRestClient client) {
			if (HasParts)
				return CreateMultipartContent(client);

			HttpContent content;
			if (IsSimpleValue()) {
				content = ValueAsString(client);
			} else {
				var format = Format;
				if (format == ContentFormat.Default)
					format = client.Settings.ContentFormat;

				if (format == ContentFormat.Default)
					throw new NotSupportedException("Invalid content format setup: the body fallbacks to default that was not set");

				var serializer = client.Settings.Serializers.FirstOrDefault(x => x.SupportedFormat == format);
				if (serializer == null)
					throw new NotSupportedException($"Unable to define a serializer for content of format {format.ToString().ToUpperInvariant()}");

				content = SerializeValue(client, serializer);
			}

			return content;
		}

		private HttpContent SerializeValue(IRestClient client, IContentSerializer serializer) {
			var s = serializer.Serialize(Value);
			var contentTypes = serializer.ContentTypes;
			return new StringContent(s, Encoding.UTF8, contentTypes[0]);
		}

		private bool IsSimpleValue() {
			return Value is string ||
			       Value is int ||
			       Value is short ||
			       Value is long ||
				   Value is double ||
				   Value is float ||
				   Value is decimal ||
				   Value is DateTime ||
				   Value is DateTimeOffset ||
				   Value is TimeSpan ||
				   Value is Guid;
		}

		private StringContent ValueAsString(IRestClient client) {
			// TODO: get this from the client's settings
			var s = Value == null ? null : Convert.ToString(Value, CultureInfo.InvariantCulture);
			return new StringContent(s, Encoding.UTF8, "text/plain");
		}

		private MultipartContent CreateMultipartContent(IRestClient client) {
			var multipart = new MultipartFormDataContent();

			foreach (var bodyPart in parts.Values) {
				HttpContent content;

				if (bodyPart is HttpFile) {
					content = ((HttpFile)bodyPart).CreateFileContent(true);
				} else {
					content = ((RequestBodyParameter)bodyPart).CreateContent(client);
				}

				multipart.Add(content);
			}

			return multipart;
		}
	}
}