using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Deveel.Web.Client {
	public static class ObjectExtensions {
		public static IEnumerable<IRequestParameter> AsParameters(this object obj, RequestParameterType type) {
			return obj.GetType().GetTypeInfo()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Select(x => new SimpleRequestParameter(type, x.Name, x.GetValue(obj, null)));
		}
	}
}