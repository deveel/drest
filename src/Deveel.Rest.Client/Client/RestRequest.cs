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

		public IRequestAuthenticator Authenticator { get; set; }

		public string Resource { get; }

		public ICollection<IRequestParameter> Parameters { get; set; }

		IEnumerable<IRequestParameter> IRestRequest.Parameters => Parameters;

		public Type ReturnedType { get; set; }

		private static string SafeValue(object value) {
			return value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		private static HttpContent MakeFileMultipart(IEnumerable<IRequestParameter> files) {
			var content = new MultipartFormDataContent();

			foreach (var file in files) {
				content.Add(file.GetFileContent(true));
			}

			return content;
		}

		internal HttpRequestMessage AsHttpRequestMessage(IRestClient client) {
			var uri = UriHelper.MakeUri(client.Settings.BaseUri, this);

			var httpRequest = new HttpRequestMessage(Method, uri);

			if ((Method == HttpMethod.Post || Method == HttpMethod.Put)) {
				if (this.HasBody()) {
					httpRequest.Content = this.Body().GetHttpContent(client);
				} else if (this.HasFiles()) {
					var files = this.Files().ToList();
					if (files.Count > 1) {
						httpRequest.Content = MakeFileMultipart(this.Files());
					} else {
						httpRequest.Content = files[0].GetFileContent(false);
					}
				}
			}

			if (this.HasHeaders()) {
				foreach (var header in this.Headers()) {
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

		public static RestRequest PostFile(string resource, RequestFile file, object routes = null, object query = null) {
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