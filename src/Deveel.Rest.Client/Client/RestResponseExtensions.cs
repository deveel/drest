using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public static class RestResponseExtensions {
		public static Task<T> GetBodyAsync<T>(this IRestResponse response) {
			return GetBodyAsync<T>(response, CancellationToken.None);
		}

		public static async Task<T> GetBodyAsync<T>(this IRestResponse response, CancellationToken cancellationToken) {
			var body = await response.GetBodyAsync(cancellationToken);
			return (T) body;
		}

		public static Task<object> GetBodyAsync(this IRestResponse response) {
			return response.GetBodyAsync(CancellationToken.None);
		}

		public static Task<ResponseFile> GetFileAsync(this IRestResponse response) {
			return response.GetBodyAsync<ResponseFile>();
		}
	}
}