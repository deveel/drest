using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public static class RestClientExtensions {
		public static Task<IRestResponse> RequestAsync(this IRestClient client, Action<IRequestBuilder> builder, 
			CancellationToken cancellationToken = default(CancellationToken)) {
			using (var request = RestRequest.Build(builder)) {
				return client.RequestAsync(request, cancellationToken);
			}
		}

		public static async Task<T> RequestAsync<T>(this IRestClient client, RestRequest request, CancellationToken cancellationToken = default(CancellationToken)) {
			if (request.Returned == null || request.Returned.ReturnType == null) {
				request.Returned = new RequestReturn(typeof(T));
			} else if (!typeof(T).GetTypeInfo().IsAssignableFrom(request.Returned.ReturnType)) {
				throw new ArgumentException($"The type {request.Returned.ReturnType} set in the request is not assignable to the expected type {typeof(T)}");
			}

			var response = await client.RequestAsync(request, cancellationToken);
			
			response.AssertSuccessful();
			if (!response.IsSuccessful())
				throw response.GetException();

			return await response.GetBodyAsync<T>(cancellationToken);
		}

		public static Task<T> RequestAsync<T>(this IRestClient client, Action<IRequestBuilder> builder,
			CancellationToken cancellationToken = default(CancellationToken)) {
			using (var request = RestRequest.Build(builder)) {
				return client.RequestAsync<T>(request, cancellationToken);
			}
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource) {
			return client.GetAsync<T>(resource, CancellationToken.None);
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource, CancellationToken cancellationToken) {
			return client.GetAsync<T>(resource, null, cancellationToken);
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource, object query, CancellationToken cancellationToken) {
			return client.GetAsync<T>(resource, null, query, cancellationToken);
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource, object query) {
			return client.GetAsync<T>(resource, null, query);
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource, object routes, object query) {
			return client.GetAsync<T>(resource, routes, query, CancellationToken.None);
		}

		public static Task<T> GetAsync<T>(this IRestClient client, string resource, object routes, object query, CancellationToken cancellationToken) {
			return client.RequestAsync<T>(RestRequest.Get<T>(resource, routes, query), cancellationToken);
		}

		public static Task DeleteAsync(this IRestClient client, string resource) {
			return client.DeleteAsync(resource, CancellationToken.None);
		}

		public static Task DeleteAsync(this IRestClient client, string resource, CancellationToken cancellationToken) {
			return client.DeleteAsync(resource, null, cancellationToken);
		}

		public static Task DeleteAsync(this IRestClient client, string resource, object query, CancellationToken cancellationToken) {
			return client.DeleteAsync(resource, null, query, cancellationToken);
		}

		public static Task DeleteAsync(this IRestClient client, string resource, object query) {
			return client.DeleteAsync(resource, null, query);
		}

		public static Task DeleteAsync(this IRestClient client, string resource, object routes, object query) {
			return client.DeleteAsync(resource, routes, query, CancellationToken.None);
		}

		public static async Task DeleteAsync(this IRestClient client, string resource, object routes, object query, CancellationToken cancellationToken) {
			var response = await client.RequestAsync(RestRequest.Delete(resource, routes, query), cancellationToken);
			response.AssertSuccessful();
		}

		public static Task PostAsync(this IRestClient client, string resource, object body, CancellationToken cancellationToken) {
			return client.PostAsync(resource, null, body, cancellationToken);
		}

		public static Task PostAsync(this IRestClient client, string resource, object query, object body, CancellationToken cancellationToken) {
			return client.PostAsync(resource, null, query, body, cancellationToken);
		}

		public static Task PostAsync(this IRestClient client, string resource, object body) {
			return client.PostAsync(resource, null, body);
		}

		public static Task PostAsync(this IRestClient client, string resource, object query, object body) {
			return client.PostAsync(resource, null, query, body);
		}

		public static Task PostAsync(this IRestClient client, string resource, object routes, object query, object body) {
			return client.PostAsync(resource, routes, query, body, CancellationToken.None);
		}

		public static async Task<T> PostAsync<T>(this IRestClient client, string resource, object routes, object query, object body, CancellationToken cancellationToken) {
			var response = await client.RequestAsync(RestRequest.Post<T>(resource, body, routes, query), cancellationToken);
			response.AssertSuccessful();
			return await response.GetBodyAsync<T>(cancellationToken);
		}

		public static Task<T> PostAsync<T>(this IRestClient client, string resource, object body, CancellationToken cancellationToken) {
			return client.PostAsync<T>(resource, null, body, cancellationToken);
		}

		public static Task<T> PostAsync<T>(this IRestClient client, string resource, object query, object body, CancellationToken cancellationToken) {
			return client.PostAsync<T>(resource, null, query, body, cancellationToken);
		}

		public static Task<T> PostAsync<T>(this IRestClient client, string resource, object body) {
			return client.PostAsync<T>(resource, null, body);
		}

		public static Task<T> PostAsync<T>(this IRestClient client, string resource, object query, object body) {
			return client.PostAsync<T>(resource, null, query, body);
		}

		public static Task<T> PostAsync<T>(this IRestClient client, string resource, object routes, object query, object body) {
			return client.PostAsync<T>(resource, routes, query, body, CancellationToken.None);
		}

		public static async Task PostAsync(this IRestClient client, string resource, object routes, object query, object body, CancellationToken cancellationToken) {
			var response = await client.RequestAsync(RestRequest.Post(resource, body, routes, query), cancellationToken);
			response.AssertSuccessful();
		}


		public static Task PutAsync(this IRestClient client, string resource, object body, CancellationToken cancellationToken) {
			return client.PutAsync(resource, null, body, cancellationToken);
		}

		public static Task PutAsync(this IRestClient client, string resource, object query, object body, CancellationToken cancellationToken) {
			return client.PutAsync(resource, null, query, body, cancellationToken);
		}

		public static Task PutAsync(this IRestClient client, string resource, object body) {
			return client.PutAsync(resource, null, body);
		}

		public static Task PutAsync(this IRestClient client, string resource, object query, object body) {
			return client.PutAsync(resource, null, query, body);
		}

		public static Task PutAsync(this IRestClient client, string resource, object routes, object query, object body) {
			return client.PutAsync(resource, routes, query, body, CancellationToken.None);
		}

		public static async Task<T> PutAsync<T>(this IRestClient client, string resource, object routes, object query, object body, CancellationToken cancellationToken) {
			var response = await client.RequestAsync(RestRequest.Put<T>(resource, body, routes, query), cancellationToken);
			response.AssertSuccessful();
			return await response.GetBodyAsync<T>(cancellationToken);
		}

		public static Task<T> PutAsync<T>(this IRestClient client, string resource, object body, CancellationToken cancellationToken) {
			return client.PutAsync<T>(resource, null, body, cancellationToken);
		}

		public static Task<T> PutAsync<T>(this IRestClient client, string resource, object query, object body, CancellationToken cancellationToken) {
			return client.PutAsync<T>(resource, null, query, body, cancellationToken);
		}

		public static Task<T> PutAsync<T>(this IRestClient client, string resource, object body) {
			return client.PutAsync<T>(resource, null, body);
		}

		public static Task<T> PutAsync<T>(this IRestClient client, string resource, object query, object body) {
			return client.PutAsync<T>(resource, null, query, body);
		}

		public static Task<T> PutAsync<T>(this IRestClient client, string resource, object routes, object query, object body) {
			return client.PutAsync<T>(resource, routes, query, body, CancellationToken.None);
		}

		public static async Task PutAsync(this IRestClient client, string resource, object routes, object query, object body, CancellationToken cancellationToken) {
			var response = await client.RequestAsync(RestRequest.Put(resource, body, routes, query), cancellationToken);
			response.AssertSuccessful();
		}
	}
}