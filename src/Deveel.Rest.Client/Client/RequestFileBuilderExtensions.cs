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

		public static IRequestFileBuilder SvgImage(this IRequestFileBuilder builder) {
			return builder.WithContentType("image/svg+xml");
		}

		public static IRequestFileBuilder AiffAudio(this IRequestFileBuilder builder) {
			return builder.WithContentType("audio/aiif");
		}

		public static IRequestFileBuilder Mp3Audio(this IRequestFileBuilder builder) {
			return builder.WithContentType("audio/mpeg");
		}

		public static IRequestFileBuilder Mp4Video(this IRequestFileBuilder builder) {
			return builder.WithContentType("video/mp4");
		}

		public static IRequestFileBuilder AviVideo(this IRequestFileBuilder builder) {
			return builder.WithContentType("video/x-msvideo");
		}

		public static IRequestFileBuilder QuickTimeVideo(this IRequestFileBuilder builder) {
			return builder.WithContentType("video/quicktime");
		}

		public static IRequestFileBuilder PdfDocument(this IRequestFileBuilder builder) {
			return builder.WithContentType("application/pdf");
		}

		public static IRequestFileBuilder PlainText(this IRequestFileBuilder builder) {
			return builder.WithContentType("text/plain");
		}

		public static IRequestFileBuilder WithFileContent(this IRequestFileBuilder builder, string filePath) {
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"Cannot find the file {filePath}.", filePath);

			var provider = new DefaultContentTypeProvider();
			var fileName = Path.GetFileName(filePath);

			var content = File.OpenRead(filePath);
			string contentType;
			if (provider.TryGetContentType(filePath, out contentType))
				builder = builder.WithContentType(contentType);

			return builder.WithFileName(fileName).WithContent(content);
		}
	}
}