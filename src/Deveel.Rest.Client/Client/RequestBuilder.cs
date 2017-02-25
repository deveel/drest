using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Deveel.Web.Client {
	class RequestBuilder : IRequestBuilder {
		private HttpMethod httpMethod;
		private string resourceName;
		private List<IRequestParameter> parameters;
		private bool? authenticate;
		private Type returnedType;
		private IRequestAuthenticator requestAuthenticator;

		public RequestBuilder() {
			parameters = new List<IRequestParameter>();
		}

		public IRequestBuilder Method(HttpMethod method) {
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			httpMethod = method;
			return this;
		}

		public IRequestBuilder To(string resource) {
			if (String.IsNullOrEmpty(resource))
				throw new ArgumentNullException(nameof(resource));

			resourceName = resource;
			return this;
		}

		public IRequestBuilder With(IRequestParameter parameter) {
			if (parameter == null)
				throw new ArgumentNullException(nameof(parameter));

			if (parameters.Any(x => x.Name == parameter.Name &&
			                        x.Type == parameter.Type))
				throw new ArgumentException($"The parameter {parameter.Name} of type {parameter.Type} already set");

			if (parameter.IsBody()) {
				if (parameters.Any(x => x.IsBody()))
					throw new ArgumentException();
			}

			parameters.Add(parameter);
			return this;
		}

		public IRequestBuilder Authenticate(bool value = true) {
			authenticate = value;
			return this;
		}

		public IRequestBuilder Returns(Type returnType) {
			returnedType = returnType;
			return this;
		}

		public IRequestBuilder UseAuthenticator(IRequestAuthenticator authenticator) {
			if (authenticator == null)
				throw new ArgumentNullException(nameof(authenticator));

			requestAuthenticator = authenticator;
			authenticate = true;
			return this;
		}

		public RestRequest Build() {
			if (httpMethod == null)
				throw new InvalidOperationException("The HTTP method is required");
			if (String.IsNullOrEmpty(resourceName))
				throw new InvalidOperationException("The resource name is required");

			var request = new RestRequest(httpMethod, resourceName) {
				Authenticate = authenticate,
				Authenticator = requestAuthenticator,
				ReturnedType = returnedType
			};

			var simpleParams = parameters.Where(x => !x.IsBody() && !x.IsFile());
			foreach (var parameter in simpleParams) {
				request.Parameters.Add(parameter);
			}

			var body = parameters.FirstOrDefault(x => x.IsBody());
			var files = parameters.Where(x => x.IsFile()).ToList();

			if (files.Count > 1) {
				if (body == null)
					body = new RequestBody();

				if (!(body is IMultipartBody))
					throw new NotSupportedException("A body was found but is not multi-partable");

				foreach (var file in files) {
					var filePart = file as IBodyPart;
					if (filePart == null)
						throw new InvalidOperationException("The file is not body part");

					((IMultipartBody)body).AddPart(filePart);
				}
			} else if (files.Count == 1) {
				request.Parameters.Add(files[0]);
			}

			if (body != null) {
				if (files.Count == 1) {
					var multipart = new RequestBody();
					multipart.AddPart(body as IBodyPart);
					multipart.AddPart(files[0] as IBodyPart);
					request.Parameters.Add(multipart);
				} else {
					request.Parameters.Add(body);
				}
			}

			return request;
		}
	}
}