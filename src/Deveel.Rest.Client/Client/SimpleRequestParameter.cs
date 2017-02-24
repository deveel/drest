using System;

namespace Deveel.Web.Client {
	public sealed class SimpleRequestParameter : IRequestParameter {
		public SimpleRequestParameter(RequestParameterType parameterType, string parameterName, object parameterValue) {
			if (parameterType == RequestParameterType.Body)
				throw new ArgumentException();
			if (String.IsNullOrEmpty(parameterName))
				throw new ArgumentNullException(nameof(parameterName));

			Type = parameterType;
			Name = parameterName;
			Value = parameterValue;
		}

		public RequestParameterType Type { get; }

		public string Name { get; }

		public object Value { get; }
	}
}