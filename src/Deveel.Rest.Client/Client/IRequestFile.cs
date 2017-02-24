using System;
using System.IO;

namespace Deveel.Web.Client {
	public interface IRequestFile : IBodyPart {
		Stream Content { get; }

		int BufferSize { get; }

		string ContentType { get; }

		string FileName { get; }
	}
}