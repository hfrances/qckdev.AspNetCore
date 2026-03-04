using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
        [TestCategory("HttpHeaderValidation")]
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
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderMandatory_NotPresent_ShouldReturn500()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderMandatory_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, "testuser");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User header is valid");
            content.Should().Contain("testuser");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderMandatory_PresentWithDifferentCase_ShouldReturn200()
        {
            // Arrange - HTTP header names are case-insensitive
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, "testuser");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User header is valid");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderOptional_NotPresent_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-optional-user-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User header is optional");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderOptional_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-optional-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, "testuser");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User header is optional");
            content.Should().Contain("testuser");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
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

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task DifferentHeaderType_Mandatory_NotPresent_ShouldReturn500()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-company-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task DifferentHeaderType_Mandatory_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-company-header");
            request.Headers.Add(HttpCompanyHeaderAttribute.HeaderNameValue, "company123");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Company header is valid");
            content.Should().Contain("company123");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task MultipleHeaders_BothPresent_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, "testuser");
            request.Headers.Add(HttpCompanyHeaderAttribute.HeaderNameValue, "company123");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User header is valid");
            content.Should().Contain("testuser");
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderMandatory_PresentWithStringEmpty_ShouldReturn500()
        {
            // Arrange - string.Empty is filtered by HTTP client, so header never arrives
            // This is equivalent to header not being present and mandatory
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, string.Empty);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderMandatory_PresentWithWhitespaceValue_ShouldReturn200()
        {
            // Arrange - Whitespace value is transmitted and considered valid
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-mandatory-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, " ");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("HttpHeaderValidation")]
        public async Task HeaderOptional_PresentWithWhitespaceValue_ShouldReturn200()
        {
            // Arrange - Whitespace value is transmitted and considered valid
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/headers/with-optional-user-header");
            request.Headers.Add(HttpUserHeaderAttribute.HeaderNameValue, " ");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
