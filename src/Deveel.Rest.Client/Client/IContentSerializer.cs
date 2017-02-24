using System;

namespace Deveel.Web.Client {
	public interface IContentSerializer {
		ContentFormat SupportedFormat { get; }

		string[] ContentTypes { get; }

		string Serialize(object obj);

		object Deserialize(Type type, string source);
	}
}