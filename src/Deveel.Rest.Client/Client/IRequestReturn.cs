using System;

namespace Deveel.Web.Client {
	public interface IRequestReturn {
		Type ReturnType { get; }

		string ContentType { get; }
	}
}