using System;

namespace Deveel.Web.Client {
	public interface IRequestParameter {
		RequestParameterType Type { get; }

		string Name { get; }

		object Value { get; }
	}
}