using System;

namespace Deveel.Web.Client {
	public interface IContentTypeProvider {
		bool TryGetContentType(string fileName, out string contentType);
	}
}