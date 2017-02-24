using System;

namespace Deveel.Web.Client {
	public class DefaultBuildContext : IBuildContext {
		public object Resolve(Type serviceType) {
			return Activator.CreateInstance(serviceType, true);
		}
	}
}