using System;

namespace Deveel.Web.Client {
	public interface IBuildContext {
		object Resolve(Type serviceType);
	}
}