using System;

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
	}
}