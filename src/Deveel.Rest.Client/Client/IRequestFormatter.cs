using System;

namespace Deveel.Web.Client {
	public interface IRequestFormatter {
		string Format(IRestRequest request);
	}
}