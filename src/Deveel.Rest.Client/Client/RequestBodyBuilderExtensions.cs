using System;

namespace Deveel.Web.Client {
	public static class RequestBodyBuilderExtensions {
		public static IRequestBodyBuilder WithJsonContent(this IRequestBodyBuilder builder, object value) {
			return builder.OfFormat(ContentFormat.Json).WithContent(value);
		}

		public static IRequestBodyBuilder WithXmlContent(this IRequestBodyBuilder builder, object value) {
			return builder.OfFormat(ContentFormat.Xml).WithContent(value);
		}

		public static IRequestBodyBuilder WithKeyValueContent(this IRequestBodyBuilder builder, object value) {
			return builder.OfFormat(ContentFormat.KeyValue).WithContent(value);
		}

		public static IRequestBodyBuilder IncludeBody(this IRequestBodyBuilder builder, string name, object value) {
			return builder.IncludePart(new RequestBody(name, value, ContentFormat.KeyValue));
		}

		public static IRequestBodyBuilder IncludeFile(this IRequestBodyBuilder builder, RequestFile file) {
			return builder.IncludePart(file);
		}
	}
}