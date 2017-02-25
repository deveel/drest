using System;

namespace Deveel.Web.Client {
	public sealed class SimpleRequestParameter : IRequestParameter {
		public SimpleRequestParameter(RequestParameterType type, string name, object value) {
			if (type == RequestParameterType.Body)
				throw new ArgumentException();
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			Type = type;
			Name = name;
			Value = value;
		}

		public RequestParameterType Type { get; }

		public string Name { get; }

		public object Value { get; }
	}
}