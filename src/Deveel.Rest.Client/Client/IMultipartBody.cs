using System;
using System.Collections.Generic;

namespace Deveel.Web.Client {
	public interface IMultipartBody : IRequestBody {
		IEnumerable<KeyValuePair<string, IBodyPart>> Parts { get; }

		void AddPart(IBodyPart part);
	}
}