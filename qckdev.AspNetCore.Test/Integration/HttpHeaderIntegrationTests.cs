using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using qckdev.AspNetCore.Test.Fixtures;
using qckdev.AspNetCore.Mvc.Headers;

namespace qckdev.AspNetCore.Test.Integration
{
    /// <summary>
    /// Integration tests for HTTP header validation through the complete HTTP pipeline.
    /// Tests the HttpHeaderValidatorMiddleware in its actual context with real endpoints.
    /// </summary>
    [TestClass]
    public class HttpHeaderIntegrationTests
    {
        private HttpHeaderTestApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new HttpHeaderTestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task HeaderNotRequired_NoEndpointAttribute_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/without-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("No header required");
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task AcceptLanguageHeader_Mandatory_NotPresent_ShouldReturn500()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-accept-language");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task AcceptLanguageHeader_Mandatory_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-accept-language");
            request.Headers.Add(HttpAcceptLanguageHeaderAttribute.HeaderNameValue, "en-US");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Accept-Language header is valid");
            content.Should().Contain("en-US");
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task AcceptLanguageHeader_Optional_NotPresent_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-optional-accept-language");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Accept-Language header is optional");
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task AcceptLanguageHeader_Optional_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-optional-accept-language");
            request.Headers.Add(HttpAcceptLanguageHeaderAttribute.HeaderNameValue, "es-ES");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Accept-Language header is optional");
            content.Should().Contain("es-ES");
        }

        [TestMethod]
        [TestCategory("AcceptLanguageHeaderValidation")]
        public async Task HeaderUnavailable_ShouldPassThrough()
        {
            // Arrange - Endpoint has header attribute but it's not available
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-unavailable-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Endpoint with unavailable header");
        }
    }
}
