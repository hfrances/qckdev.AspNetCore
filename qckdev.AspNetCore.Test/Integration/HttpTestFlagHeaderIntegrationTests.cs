using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using qckdev.AspNetCore.Test.Fixtures;

namespace qckdev.AspNetCore.Test.Integration
{
    /// <summary>
    /// Integration tests for test-flag custom header validation.
    /// Tests the HttpHeaderValidatorMiddleware with a custom test-flag header.
    /// </summary>
    [TestClass]
    public class HttpTestFlagHeaderIntegrationTests
    {
        private TestFlagHeaderTestApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new TestFlagHeaderTestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task NoHeader_EndpointWithoutAttribute_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/no-header");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("No test-flag header required");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_NotPresent_ShouldReturn500()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_PresentWithValue_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "feature-enabled");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is required");
            content.Should().Contain("feature-enabled");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_PresentWithNumericValue_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is required");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_DifferentCase_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "enabled");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is required");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task OptionalFlag_NotPresent_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/optional-flag");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is optional");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task OptionalFlag_Present_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/optional-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "true");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is optional");
            content.Should().Contain("true");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task UnavailableFlag_ShouldPassThrough()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/unavailable-flag");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Endpoint with unavailable test-flag header");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_WithMultipleHeaders_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "active");
            request.Headers.Add("x-custom-header", "value");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is required");
            content.Should().Contain("active");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task OptionalFlag_WithBooleanValue_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/optional-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, "false");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test-flag header is optional");
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_PresentWithStringEmpty_ShouldReturn500()
        {
            // Arrange - string.Empty is filtered by HTTP client, so header never arrives
            // This is equivalent to header not being present and mandatory
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, string.Empty);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task MandatoryFlag_PresentWithWhitespaceValue_ShouldReturn200()
        {
            // Arrange - Whitespace value is transmitted and considered valid
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/mandatory-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, " ");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("TestFlagHeaderValidation")]
        public async Task OptionalFlag_PresentWithWhitespaceValue_ShouldReturn200()
        {
            // Arrange - Whitespace value is transmitted and considered valid
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/testflag/optional-flag");
            request.Headers.Add(HttpTestFlagHeaderAttribute.HeaderNameValue, " ");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
