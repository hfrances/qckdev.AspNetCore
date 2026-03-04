using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.AspNetCore.Test.Fixtures;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Integration
{
    [TestClass]
    public class ApiControllerIntegrationTests
    {
        private TestApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task HealthEndpoint_Returns200()
        {
            // Act
            var response = await _client.GetAsync("/api/test/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("healthy");
        }

        [TestMethod]
        public async Task SendRequest_WithValidMediatRRequest_ReturnsResponseData()
        {
            // Arrange
            var requestBody = JsonSerializer.Serialize(new TestGetDataQuery(42));
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/test/send-request", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("\"id\":42");
        }

        [TestMethod]
        public async Task SendRequest_WithValidMediatRCommand_ReturnsOk()
        {
            // Arrange
            var requestBody = JsonSerializer.Serialize(new TestVoidCommand("test message"));
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/test/send-void", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task SendRequest_WithInvalidRequest_ReturnBadRequestOrError()
        {
            // Arrange
            var invalidJson = "{ invalid json";
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/test/send-request", content);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task MultipleRequests_ExecuteSequentially()
        {
            // Act
            var response1 = await _client.GetAsync("/api/test/health");
            var response2 = await _client.GetAsync("/api/test/health");
            var response3 = await _client.GetAsync("/api/test/health");

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            response3.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task ResponseHeaders_ContainsContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/test/health");

            // Assert
            response.Content.Headers.ContentType.Should().NotBeNull();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
