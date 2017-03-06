using System;

namespace Deveel.Web.Client {
	public class RequestReturn : IRequestReturn {
		public RequestReturn(Type returnType) 
			: this(returnType, null) {
		}

		public RequestReturn(Type returnType, string contentType) {
			if (returnType == null)
				throw new ArgumentNullException(nameof(returnType));

			ReturnType = returnType;
			ContentType = contentType;
		}

		public Type ReturnType { get; }

		public string ContentType { get; set; }

		public static RequestReturn Void => new RequestReturn(typeof(void));

		public static RequestReturn File(string contentType) => new RequestReturn(typeof(RequestFile), contentType);

		public static RequestReturn File() => File(null);

		public static RequestReturn Object(Type type, string contentType) => new RequestReturn(type, contentType);

		public static RequestReturn Object(Type type) => new RequestReturn(type, null);
	}
}