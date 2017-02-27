using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Deveel.Web.Client {
	public class XmlContentSerializer : IContentSerializer {
		public XmlContentSerializer(XmlReaderSettings readerSettings, XmlWriterSettings writerSettings) {
			ReaderSettings = readerSettings;
			WriterSettings = writerSettings;
		}

		public XmlContentSerializer()
			: this(new XmlReaderSettings(), new XmlWriterSettings()) {
		}

		public XmlWriterSettings WriterSettings { get; }

		public XmlReaderSettings ReaderSettings { get; }

		ContentFormat IContentSerializer.SupportedFormat => ContentFormat.Xml;

		string[] IContentSerializer.ContentTypes => new[] {"text/xml"};

		public string Serialize(IRestClient client, object obj) {
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			var serializer = new XmlSerializer(obj.GetType());

			using (var writer = new StringWriter()) {
				using (var xmlWriter = XmlWriter.Create(writer, WriterSettings)) {
					serializer.Serialize(xmlWriter, obj);
				}

				writer.Flush();
				return writer.ToString();
			}
		}

		public object Deserialize(IRestClient client, Type type, string source) {
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			var serializer = new XmlSerializer(type);

			using (var reader = new StringReader(source)) {
				using (var xmlReader = XmlReader.Create(reader, ReaderSettings)) {
					if (!serializer.CanDeserialize(xmlReader))
						throw new InvalidOperationException();

					return serializer.Deserialize(xmlReader);
				}
			}
		}
	}
}