using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public class DefaultHttpClient : IHttpClient {
		private readonly HttpClient httpClient;

		public DefaultHttpClient(HttpClient httpClient) {
			this.httpClient = httpClient;
		}

		public DefaultHttpClient(HttpMessageHandler messageHandler)
			: this(new HttpClient(messageHandler)) {
		}

		public DefaultHttpClient()
			: this(new HttpClient()) {
		}

		public Uri BaseAddress {
			get { return httpClient.BaseAddress; }
			set { httpClient.BaseAddress = value;}
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken) {
			return httpClient.SendAsync(message, cancellationToken);
		}

		public void AddDefaultHeader(string key, object value) {
			if (value is IEnumerable<string>) {
				httpClient.DefaultRequestHeaders.Add(key, (IEnumerable<string>)value);
			} else {
				httpClient.DefaultRequestHeaders.Add(key, SafeValue(value));
			}
		}

		private string SafeValue(object value) {
			if (value is string)
				return (string) value;
			if (value == null)
				return "";

			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
	}
}