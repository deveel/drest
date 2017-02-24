using System;

namespace Deveel.Web.Client {
	public interface IRequestBodyBuilder {
		IRequestBodyBuilder Named(string name);

		IRequestBodyBuilder WithContent(object value);

		IRequestBodyBuilder IncludePart(IBodyPart part);

		IRequestBodyBuilder OfFormat(ContentFormat format);

		IRequestBody Build();
	}
}