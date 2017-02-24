using System;
using System.Collections.Generic;

namespace Deveel.Web.Client {
	class RequestBodyBuilder : IRequestBodyBuilder {
		private string bodyName;
		private object content;
		private List<IBodyPart> parts;
		private bool multipart;
		private ContentFormat? bodyFormat;

		public IRequestBodyBuilder Named(string name) {
			bodyName = name;
			return this;
		}

		public IRequestBodyBuilder WithContent(object value) {
			AssertNotMultipart();

			content = value;
			return this;
		}

		private void AssertNotMultipart() {
			if (multipart)
				throw new InvalidOperationException("The body is multiparted");
		}

		private void AssertMultipartable() {
			if (bodyFormat != null ||
				content != null)
				throw new InvalidOperationException("Cannot form a multi-part body");
		}

		public IRequestBodyBuilder IncludePart(IBodyPart part) {
			AssertMultipartable();

			if (parts == null)
				parts = new List<IBodyPart>();

			parts.Add(part);
			multipart = true;
			return this;
		}

		public IRequestBody Build() {
			RequestBody body;
			if (multipart) {
				body = new RequestBody(bodyName);
			} else {
				body = new RequestBody(bodyName, content, bodyFormat ?? ContentFormat.Default);
			}

			if (multipart) {
				if (parts != null) {
					foreach (var part in parts) {
						body.AddPart(part);
					}
				}
			}

			return body;
		}

		public IRequestBodyBuilder OfFormat(ContentFormat format) {
			AssertNotMultipart();

			bodyFormat = format;
			return this;
		}
	}
}