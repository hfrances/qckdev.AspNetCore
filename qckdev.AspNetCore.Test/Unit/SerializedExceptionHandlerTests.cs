using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using qckdev.AspNetCore.Exceptions;
using qckdev.AspNetCore.Middlewares;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Unit
{
    [TestClass]
    public class SerializedExceptionHandlerTests
    {
        private Mock<ILogger<SerializedExceptionHandlerResponseMiddleware>> _loggerMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SerializedExceptionHandlerResponseMiddleware>>();
        }

        [TestMethod]
        public async Task Middleware_WithNoException_CallsNextMiddleware()
        {
            // Arrange
            var nextCalled = false;
            var nextDelegate = new RequestDelegate(async _ => 
            {
                nextCalled = true;
                await Task.CompletedTask;
            });

            var httpContext = new DefaultHttpContext();
            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            nextCalled.Should().BeTrue("Next middleware should be called when no exception occurs");
        }
        public async Task Middleware_WithHttpHandledException_ReturnsCorrectStatusCode()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new HttpHandledException(HttpStatusCode.NotFound, "Not found"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            httpContext.Response.ContentType.Should().Contain("application/json");
        }

        [TestMethod]
        public async Task Middleware_WithApiException_SerializesErrorResponse()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new ApiException(HttpStatusCode.BadRequest, "Validation.Failed"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            content.Should().NotBeEmpty();
            (content.Contains("error") || content.Contains("message")).Should().BeTrue();
        }

        [TestMethod]
        public async Task Middleware_WithGenericException_Returns500StatusCode()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new InvalidOperationException("Unexpected error"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Middleware_WithException_SetsJsonContentType()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new HttpHandledException(HttpStatusCode.Conflict, "Conflict occurred"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            httpContext.Response.ContentType.Should().Be("application/json");
        }

        [TestMethod]
        public async Task Middleware_WithApiException_LogsError()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new ApiException(HttpStatusCode.BadRequest, "Validation.Failed"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<ApiException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Middleware_WithDifferentHttpStatusCodes_ReturnsCorrectCodes()
        {
            // Test multiple status codes
            var testCases = new[]
            {
                (HttpStatusCode.BadRequest, 400),
                (HttpStatusCode.Unauthorized, 401),
                (HttpStatusCode.Forbidden, 403),
                (HttpStatusCode.NotFound, 404),
                (HttpStatusCode.Conflict, 409),
            };

            foreach (var (statusCode, expectedCode) in testCases)
            {
                // Arrange
                var nextDelegate = new RequestDelegate(_ => 
                    throw new HttpHandledException(statusCode, $"Error {statusCode}"));

                var httpContext = new DefaultHttpContext();
                httpContext.Response.Body = new MemoryStream();

                var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

                // Act
                await middleware.Invoke(httpContext);

                // Assert
                httpContext.Response.StatusCode.Should().Be(expectedCode, 
                    $"Status code for {statusCode} should be {expectedCode}");
            }
        }

        [TestMethod]
        public async Task Middleware_WithException_WritesResponseBody()
        {
            // Arrange
            var nextDelegate = new RequestDelegate(_ => 
                throw new HttpHandledException(HttpStatusCode.BadRequest, "Invalid request"));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var middleware = new SerializedExceptionHandlerResponseMiddleware(nextDelegate, _loggerMock.Object);

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            httpContext.Response.Body.Length.Should().BeGreaterThan(0, "Response body should contain error details");
        }
    }
}
