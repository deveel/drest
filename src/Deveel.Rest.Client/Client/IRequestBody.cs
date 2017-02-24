using System;

namespace Deveel.Web.Client {
	public interface IRequestBody : IBodyPart {
		ContentFormat Format { get; }
	}
}