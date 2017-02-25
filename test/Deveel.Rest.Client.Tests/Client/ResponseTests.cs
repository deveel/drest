﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
			httpClient.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(message => message.RequestUri.ToString() == "http://example.com/foo"), CancellationToken.None))
				.Returns((HttpRequestMessage message, CancellationToken token) => {
					var input = JsonConvert.DeserializeObject<dynamic>(message.Content.ReadAsStringAsync().Result);
					var httpResponse = new HttpResponseMessage(HttpStatusCode.OK) {
						RequestMessage = message,
						Content = new StringContent(JsonConvert.SerializeObject(new {
							b = (int)input.a + 2,
						}), Encoding.UTF8, "application/json")
					};
					return Task.FromResult(httpResponse);
				});

			var client = new RestClient(httpClient.Object, new RestClientSettings {
				BaseUri = new Uri("http://example.com")
			});

			var response = client.RequestAsync(request => request.Post().To("foo").WithBody(new {a = 1}).Returns<dynamic>()).Result;

			Assert.IsNotNull(response);
			Assert.IsNotNull(response.Request);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var body = response.GetBodyAsync<dynamic>().Result;

			Assert.IsNotNull(body);
			Assert.AreEqual(3, (int) body.b);
		}

		[Test]
		public void SendOneFile() {
			var httpClient = new Mock<IHttpClient>()
				.SetupAllProperties();
			httpClient.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(message => message.RequestUri.ToString() == "http://example.com/foo"), CancellationToken.None))
				.Returns((HttpRequestMessage message, CancellationToken token) => {
					var input = message.Content.ReadAsByteArrayAsync().Result;
					Assert.IsNotNull(input);
					Assert.IsNotEmpty(input);

					var httpResponse = new HttpResponseMessage(HttpStatusCode.OK) {
						RequestMessage = message
					};
					return Task.FromResult(httpResponse);
				});

			var client = new RestClient(httpClient.Object, new RestClientSettings {
				BaseUri = new Uri("http://example.com")
			});

			var file = new RequestFile("a", "a.png", new MemoryStream(new byte[] {0, 0, 2, 8, 9, 20}));
			var response = client.RequestAsync(builder => builder.To("foo").Post().WithFile(file)).Result;

			Assert.IsNotNull(response);
			Assert.DoesNotThrow(() => response.AssertSuccessful());
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}
	}
}