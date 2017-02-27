using System;

namespace Deveel.Web.Client {
	public interface IContentSerializer {
		ContentFormat SupportedFormat { get; }

		string[] ContentTypes { get; }

		string Serialize(IRestClient client, object obj);

		object Deserialize(IRestClient client, Type type, string source);
	}
}