using System;
using System.IO;
using System.Threading.Tasks;

namespace Deveel.Web.Client {
	public sealed class ResponseFile {
		private readonly Func<Task<Stream>> contentRead;

		internal ResponseFile(Func<Task<Stream>> contentRead, string contentType, long? contentLength) {
			this.contentRead = contentRead;
			ContentType = contentType;
			ContentLength = contentLength;
		}

		public string ContentType { get; }

		public long? ContentLength { get; }

		public Task<Stream> GetContentAsync() {
			return contentRead();
		}
	}
}