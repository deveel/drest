using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Deveel.Web.Client {
	public sealed class RestRequest : IRestRequest {
		public RestRequest(HttpMethod method, string resource) {
			Method = method;
			Resource = resource;
			Parameters = new List<IRequestParameter>();

			Authenticate = true;
		}

		public HttpMethod Method { get; }

		public bool Authenticate { get; set; }

		public string Resource { get; }

		public ICollection<IRequestParameter> Parameters { get; set; }

		IEnumerable<IRequestParameter> IRestRequest.Parameters => Parameters;

		public bool HasQuery => Parameters != null && Parameters.Any(x => x.ParameterType == RequestParameterType.QueryString);

		public IDictionary<string, object> Query
			=> Parameters.Where(x => x.ParameterType == RequestParameterType.QueryString)
				.ToDictionary(x => x.ParameterName, y => y.ParameterValue, StringComparer.OrdinalIgnoreCase);

		public IDictionary<string, object> Route => Parameters.Where(x => x.ParameterType == RequestParameterType.Route)
			.ToDictionary(x => x.ParameterName, y => y.ParameterValue, StringComparer.OrdinalIgnoreCase);

		public bool HasBody => Parameters != null && Parameters.Any(x => x.ParameterType == RequestParameterType.Body);

		public RequestBodyParameter Body => Parameters.OfType<RequestBodyParameter>().FirstOrDefault();

		public bool HasHeaders => Parameters != null && Parameters.Any(x => x.ParameterType == RequestParameterType.Header);

		public IDictionary<string, object> Headers => Parameters.Where(x => x.ParameterType == RequestParameterType.Header)
			.ToDictionary(x => x.ParameterName, y => y.ParameterValue, StringComparer.OrdinalIgnoreCase);

		public Type ReturnedType { get; set; }

		private static string MakeQuery(RestRequest request) {
			var pairs = request.Query.Select(pair => $"{pair.Key}={SafeValue(pair.Value)}");
			return String.Join("&", pairs);
		}

		private static string SafeValue(object value) {
			return value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		private static string MakePath(RestRequest request) {
			var resource = request.Resource;
			foreach (var pair in request.Route) {
				var key = new StringBuilder().Append("{").Append(pair.Key).Append("}").ToString();
				resource = resource.Replace(key, SafeValue(pair.Value));
			}

			return resource;
		}

		internal HttpRequestMessage AsHttpRequestMessage(IRestClient client) {
			var uriBuilder = new UriBuilder(client.Settings.BaseUri);
			uriBuilder.Path = MakePath(this);

			if (HasQuery) {
				uriBuilder.Query = MakeQuery(this);
			}

			var httpRequest = new HttpRequestMessage(Method, uriBuilder.Uri);

			if ((Method == HttpMethod.Post || Method == HttpMethod.Put) && HasBody) {
				httpRequest.Content = Body.CreateContent(client);
			}

			if (HasHeaders) {
				foreach (var header in Headers) {
					httpRequest.Headers.Add(header.Key, SafeValue(header.Value));
				}
			}

			return httpRequest;
		}

		public static RestRequest Get<T>(string resource, object routes = null, object query = null) {
			return Build(builder => builder
				.Get()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.Returns<T>());
		}

		public static RestRequest Post(string resource, object body, object routes = null, object query = null) {
			return Build(builder => builder
				.Post()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.WithBody(body));
		}

		public static RestRequest Post<T>(string resource, object body, object routes = null, object query = null) {
			return Build(builder => builder
				.Post()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.WithBody(body)
				.Returns<T>());
		}

		public static RestRequest PostFile(string resource, HttpFile file, object routes = null, object query = null) {
			return Build(builder => builder
				.Post()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.With(file));
		}

		public static RestRequest Put<T>(string resource, object body, object routes = null, object query = null) {
			return Build(builder => builder
				.Put()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.WithBody(body)
				.Returns<T>());
		}

		public static RestRequest Put(string resource, object body, object routes = null, object query = null) {
			return Build(builder => builder
				.Put()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query)
				.WithBody(body));
		}

		public static RestRequest Delete(string resource, object routes = null, object query = null) {
			return Build(builder => builder
				.Delete()
				.To(resource)
				.WithRoutes(routes)
				.WithQueryStrings(query));
		}

		public static RestRequest Build(Action<IRequestBuilder> builder) {
			var model = Model(builder);
			return model.Build();
		}

		public static IRequestBuilder Model(Action<IRequestBuilder> builder) {
			var model = new RequestBuilder();
			builder(model);
			return model;
		}
	}
}