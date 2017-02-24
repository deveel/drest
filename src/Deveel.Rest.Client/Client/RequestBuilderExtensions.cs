using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Deveel.Web.Client {
	public static class RequestBuilderExtensions {
		public static IRequestBuilder Method(this IRequestBuilder builder, string method) {
			if (String.IsNullOrEmpty(method))
				throw new ArgumentNullException(nameof(method));

			HttpMethod httpMethod;
			switch (method.ToUpperInvariant()) {
				case "GET":
					httpMethod = HttpMethod.Get;
					break;
				case "POST":
					httpMethod = HttpMethod.Post;
					break;
				case "DELETE":
					httpMethod = HttpMethod.Delete;
					break;
				case "PUT":
					httpMethod = HttpMethod.Put;
					break;
				case "HEAD":
					httpMethod = HttpMethod.Head;
					break;
				case "OPTIONS":
					httpMethod = HttpMethod.Options;
					break;
				default:
					throw new ArgumentException($"The string {method} is not a valid HTTP method");
			}

			return builder.Method(httpMethod);
		}

		public static IRequestBuilder Get(this IRequestBuilder builder) {
			return builder.Method(HttpMethod.Get);
		}

		public static IRequestBuilder Post(this IRequestBuilder builder) {
			return builder.Method(HttpMethod.Post);
		}

		public static IRequestBuilder Put(this IRequestBuilder builder) {
			return builder.Method(HttpMethod.Put);
		}

		public static IRequestBuilder Delete(this IRequestBuilder builder) {
			return builder.Method(HttpMethod.Delete);
		}

		public static IRequestBuilder To(this IRequestBuilder builder, string format, params object[] args) {
			if (String.IsNullOrEmpty(format))
				throw new ArgumentNullException(nameof(format));
			if (args == null)
				throw new ArgumentNullException(nameof(args));

			for (int i = 0; i < args.Length; i++) {
				builder = builder.WithRoute(i.ToString(), args[i]);
			}

			return builder.To(format);
		}

		public static IRequestBuilder With(this IRequestBuilder builder, params IRequestParameter[] parameters) {
			foreach (var parameter in parameters) {
				builder = builder.With(parameter);
			}

			return builder;
		}

		public static IRequestBuilder With(this IRequestBuilder builder, RequestParameterType type, string name, object value) {
			IRequestParameter parameter;
			if (type == RequestParameterType.Body) {
				parameter = new RequestBodyParameter(name, value);
			} else {
				if (String.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				parameter = new SimpleRequestParameter(type, name, value);
			}

			return builder.With(parameter);
		}

		public static IRequestBuilder WithRoute(this IRequestBuilder builder, string name, object value) {
			return builder.With(RequestParameterType.Route, name, value);
		}

		public static IRequestBuilder WithQueryString(this IRequestBuilder builder, string name, object value) {
			return builder.With(RequestParameterType.QueryString, name, value);
		}

		public static IRequestBuilder WithHeader(this IRequestBuilder builder, string name, object value) {
			return builder.With(RequestParameterType.Header, name, value);
		}

		public static IRequestBuilder With(this IRequestBuilder builder, RequestParameterType type, IEnumerable<KeyValuePair<string, object>> values) {
			foreach (var pair in values) {
				builder = builder.With(type, pair.Key, pair.Value);
			}

			return builder;
		}

		public static IRequestBuilder WithQueryStrings(this IRequestBuilder builder, IEnumerable<KeyValuePair<string, object>> values) {
			return builder.With(RequestParameterType.QueryString, values);
		}

		public static IRequestBuilder WithRoutes(this IRequestBuilder builder, IEnumerable<KeyValuePair<string, object>> values) {
			return builder.With(RequestParameterType.Route, values);
		}

		public static IRequestBuilder WithHeaders(this IRequestBuilder builder, IEnumerable<KeyValuePair<string, object>> values) {
			return builder.With(RequestParameterType.Header, values);
		}

		public static IRequestBuilder With(this IRequestBuilder builder, RequestParameterType type, object values) {
			if (values != null)
				builder = builder.With(values.AsParameters(type).ToArray());

			return builder;
		}

		public static IRequestBuilder WithQueryStrings(this IRequestBuilder builder, object values) {
			return builder.With(RequestParameterType.QueryString, values);
		}

		public static IRequestBuilder WithHeaders(this IRequestBuilder builder, object values) {
			return builder.With(RequestParameterType.Header, values);
		}

		public static IRequestBuilder WithRoutes(this IRequestBuilder builder, object values) {
			return builder.With(RequestParameterType.Route, values);
		}

		public static IRequestBuilder WithBody(this IRequestBuilder builder, string name, object value) {
			return builder.With(RequestParameterType.Body, name, value);
		}

		public static IRequestBuilder WithBody(this IRequestBuilder builder, object value) {
			return builder.WithBody(null, value);
		}

		public static IRequestBuilder Returns<T>(this IRequestBuilder builder) {
			return builder.Returns(typeof(T));
		}
	}
}