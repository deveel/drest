using System;
using System.IO;

namespace Deveel.Web.Client {
	public static class RequestFileBuilderExtensions {
		public static IRequestFileBuilder JpegImage(this IRequestFileBuilder builder) {
			return builder.WithContentType("image/jpeg");
		}

		public static IRequestFileBuilder PngImage(this IRequestFileBuilder builder) {
			return builder.WithContentType("image/png");
		}

		public static IRequestFileBuilder TiffImage(this IRequestFileBuilder builder) {
			return builder.WithContentType("image/tiff");
		}

		public static IRequestFileBuilder GifImage(this IRequestFileBuilder builder) {
			return builder.WithContentType("image/gif");
		}

		public static IRequestFileBuilder WithFileContent(this IRequestFileBuilder builder, string fileName) {
			if (!File.Exists(fileName))
				throw new FileNotFoundException($"Cannot find the file {fileName}.", fileName);

			var provider = new DefaultContentTypeProvider();
			var content = File.OpenRead(fileName);
			string contentType;
			if (provider.TryGetContentType(fileName, out contentType))
				builder = builder.WithContentType(contentType);

			return builder.WithContent(content);
		}
	}
}