using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Deveel.Web.Client {
	public class RequestFile : IRequestFile {
		public RequestFile(string name, string fileName, Stream content) 
			: this(name, fileName, null, content) {
		}

		public RequestFile(string name, string fileName, string contentType, Stream content) {
			if (content == null)
				throw new ArgumentNullException(nameof(content));
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException(nameof(fileName));

			Name = name;
			FileName = fileName;
			Content = content;
			ContentType = contentType;
		}

		RequestParameterType IRequestParameter.Type => RequestParameterType.File;

		public string Name { get; }

		public string FileName { get; }

		public Stream Content { get; }

		object IRequestParameter.Value => Content;

		public string ContentType { get; set; }

		public int BufferSize { get; set; } = 2048;

		internal static HttpContent CreateFileContent(IRequestFile file, bool inMultipart = false) {
			HttpContent content = new StreamContent(file.Content, file.BufferSize);

			content.Headers.ContentLength = file.Content.Length;

			if (!String.IsNullOrEmpty(file.FileName))
				content.Headers.ContentDisposition.FileName = file.FileName;

			if (!String.IsNullOrEmpty(file.ContentType))
				content.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

			if (!inMultipart) {
				var multipart = new MultipartFormDataContent();
				multipart.Add(content);
				content = multipart;
			}

			return content;
		}

		internal static HttpContent CreateFileContent(IRequestParameter parameter, bool inMultipart = false) {
			if (parameter is IRequestFile)
				return CreateFileContent((IRequestFile) parameter, inMultipart);

			var stream = parameter.Value as Stream;
			if (stream == null)
				throw new InvalidOperationException("A file parameter must have a stream value");

			HttpContent content = new StreamContent(stream, 2048);

			if (!inMultipart)
				content = new MultipartFormDataContent {
					{content,parameter.Name }
				};

			return content;
		}

		internal HttpContent CreateFileContent(bool inMultipart = false) {
			return CreateFileContent(this, inMultipart);
		}
	}
}