using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Deveel.Web.Client {
	[TestFixture]
	public class ResponseTests {
		[Test]
		public void SuccessfulResponse() {
			var httpClient = new Mock<IHttpClient>()
				.SetupAllProperties();
			httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), CancellationToken.None))
				.Returns((HttpRequestMessage message, CancellationToken token) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
					RequestMessage = message,
					Content = new StringContent(JsonConvert.SerializeObject(new {
						b = 2
					}), Encoding.UTF8, "application/json")
				}));

			var client= new RestClient(httpClient.Object, new RestClientSettings {
				BaseUri = new Uri("http://example.com"),
				Serializers = new List<IContentSerializer> {
					new JsonContentSerializer()
				}
			});

			var response = client.RequestAsync(request => request.Post().To("foo").WithBody(new {a = 1}).Returns<dynamic>()).Result;

			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var body = response.GetBodyAsync<dynamic>().Result;

			Assert.IsNotNull(body);
			Assert.AreEqual(2, (int) body.b);
		}
	}
}