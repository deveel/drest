using System;
using System.IO;

namespace Deveel.Web.Client {
	class RequestFileBuilder : IRequestFileBuilder {
		private string bodyName;
		private string bodyFileName;
		private string bodyContentType;
		private Stream bodyContent;

		public IRequestFileBuilder Named(string name) {
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			bodyName = name;
			return this;
		}

		public IRequestFileBuilder WithFileName(string fileName) {
			bodyFileName = fileName;
			return this;
		}

		public IRequestFileBuilder WithContentType(string contentType) {
			bodyContentType = contentType;
			return this;
		}

		public IRequestFileBuilder WithContent(Stream content) {
			if (content == null)
				throw new ArgumentNullException(nameof(content));
			if (!content.CanRead)
				throw new ArgumentException("The stream is not readable");

			bodyContent = content;
			return this;
		}

		public IRequestFile Build() {
			if (String.IsNullOrEmpty(bodyName))
				throw new InvalidOperationException("The name is required");
			if (bodyContent == null)
				throw new InvalidOperationException("A content is required");

			return new RequestFile(bodyName, bodyFileName, bodyContentType, bodyContent);
		}
	}
}