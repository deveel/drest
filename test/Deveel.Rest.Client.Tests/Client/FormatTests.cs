using System;
using System.IO;

using NUnit.Framework;

namespace Deveel.Web.Client {
	[TestFixture]
	public class FormatTests {
		[Test]
		public void FormatDeleteRequestAsHttp() {
			var client = RestClient.Build("http://api.example.com");
			var request = RestRequest.Build(builder => builder.Delete().To("user").WithQueryString("id", 22));

			var httpString = request.ToHttpString(client);

			Assert.IsNotNull(httpString);
		}

		[Test]
		public void FormatWithFileAndBody() {
			var client = RestClient.Build("http://api.example.com");
			var request = RestRequest.Build(builder => builder.Post().To("user")
				.WithBody(new RequestBody("data", new { a = 1, b = "two" }, ContentFormat.KeyValue))
				.WithFile(new RequestFile("file", "file.png", "image/png", new MemoryStream()))
				.Returns<dynamic>());

			var httpString = request.ToHttpString(client);

			Assert.IsNotNull(httpString);
		}
	}
}