using System;

namespace Deveel.Web.Client {
	public sealed class SimpleRequestParameter : IRequestParameter {
		public SimpleRequestParameter(RequestParameterType parameterType, string parameterName, object parameterValue) {
			if (parameterType == RequestParameterType.Body)
				throw new ArgumentException();
			if (String.IsNullOrEmpty(parameterName))
				throw new ArgumentNullException(nameof(parameterName));

			ParameterType = parameterType;
			ParameterName = parameterName;
			ParameterValue = parameterValue;
		}

		public RequestParameterType ParameterType { get; }

		public string ParameterName { get; }

		public object ParameterValue { get; }
	}
}