using System;

namespace Deveel.Web.Client {
	public interface IMultipartBodyParameter : IRequestParameter {
		void AddPart(RequestBodyParameter part);

		void AddFile(HttpFile file);
	}
}