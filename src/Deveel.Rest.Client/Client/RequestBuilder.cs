using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Deveel.Web.Client {
	class RequestBuilder : IRequestBuilder {
		private HttpMethod httpMethod;
		private string resourceName;
		private List<IRequestParameter> parameters;
		private bool authenticate;
		private Type returnedType;
		private List<HttpFile> files;
		private IRequestAuthenticator requestAuthenticator;

		public RequestBuilder() {
			parameters = new List<IRequestParameter>();
			files = new List<HttpFile>();
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

		private void AddBody(IRequestParameter parameter) {
			var body = parameters.FirstOrDefault(x => x.ParameterType == RequestParameterType.Body);
			if (body == null) {
				parameters.Add(parameter);
			} else {
				if (!(body is IMultipartBodyParameter))
					throw new NotSupportedException("The parent body is not multiparted");
				if (!(parameter is RequestBodyParameter))
					throw new ArgumentException("The input is not a valid body part");

				((IMultipartBodyParameter)body).AddPart((RequestBodyParameter) parameter);
			}
		}

		private void AddFile(HttpFile file) {
			var body = parameters.FirstOrDefault(x => x.ParameterType == RequestParameterType.Body);
			if (body == null) {
				files.Add(file);
			} else if (!(body is IMultipartBodyParameter)) {
				throw new NotSupportedException("The parent body is not multiparted");
			} else {
				((IMultipartBodyParameter)body).AddFile(file);
			}
		}

		public IRequestBuilder With(IRequestParameter parameter) {
			if (parameter == null)
				throw new ArgumentNullException(nameof(parameter));

			if (parameters.Any(x => x.ParameterName == parameter.ParameterName &&
			                        x.ParameterType == parameter.ParameterType))
				throw new ArgumentException($"The parameter {parameter.ParameterName} of type {parameter.ParameterType} already set");

			if (parameter.ParameterType == RequestParameterType.Body) {
				AddBody(parameter);
			} else {
				parameters.Add(parameter);
			}

			return this;
		}

		public IRequestBuilder With(HttpFile file) {
			AddFile(file);
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

			foreach (var parameter in parameters.Where(x => x.ParameterType != RequestParameterType.Body)) {
				request.Parameters.Add(parameter);
			}

			var body = parameters.FirstOrDefault(x => x.ParameterType == RequestParameterType.Body);
			if (files.Count > 0) {
				if (body == null) {
					body = new RequestBodyParameter();
				}

				if (!(body is IMultipartBodyParameter))
					throw new NotSupportedException("A body was found but is not multi-partable");

				foreach (var file in files) {
					((IMultipartBodyParameter)body).AddFile(file);
				}
			}

			if (body != null)
				request.Parameters.Add(body);

			return request;
		}
	}
}