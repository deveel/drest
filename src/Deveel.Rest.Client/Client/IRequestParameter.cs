using System;

namespace Deveel.Web.Client {
	public interface IRequestParameter {
		RequestParameterType ParameterType { get; }

		string ParameterName { get; }

		object ParameterValue { get; }
	}
}