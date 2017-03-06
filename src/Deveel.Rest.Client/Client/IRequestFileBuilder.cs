using System;
using System.IO;

namespace Deveel.Web.Client {
	public interface IRequestFileBuilder {
		IRequestFileBuilder Named(string name);

		IRequestFileBuilder WithFileName(string fileName);

		IRequestFileBuilder WithContentType(string contentType);

		IRequestFileBuilder WithContent(Stream content);

		IRequestFile Build();
	}
}