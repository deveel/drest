using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Deveel.Web.Client {
	public class HttpFile : IBodyPart {
		public HttpFile(string name, string fileName, Stream content) {
			if (content == null)
				throw new ArgumentNullException(nameof(content));
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException(nameof(fileName));

			Name = name;
			FileName = fileName;
			Content = content;
		}

		public string Name { get; }

		public string FileName { get; }

		public Stream Content { get; }

		public string ContentType { get; set; }

		public int BufferSize { get; set; } = 2048;

		internal HttpContent CreateFileContent(bool inMultipart = false) {
			HttpContent content = new StreamContent(Content, BufferSize);

			if (!String.IsNullOrEmpty(ContentType))
				content.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);

			if (!inMultipart) {
				content = new MultipartFormDataContent {
					{content, Name, FileName}
				};
			}

			return content;
		}
	}
}