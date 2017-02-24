using System;
using System.Collections.Generic;

namespace Deveel.Web.Client {
	public interface IRestClientSettings {
		IEnumerable<IContentSerializer> Serializers { get; }

		ContentFormat ContentFormat { get; }

		IEnumerable<KeyValuePair<string, object>> DefaultHeaders { get; }

		string BaseUri { get; }
	}
}