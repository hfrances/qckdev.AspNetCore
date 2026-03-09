using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using qckdev.AspNetCore.Mvc.Headers;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Web application factory for integration testing
    /// </summary>
    public class TestApplicationFactory : IDisposable
    {
        private TestServer? _testServer;

        public TestApplicationFactory()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(TestApiController).Assembly);
                    services.AddMediatRCompatibility(typeof(TestGetDataQueryHandler).Assembly);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                }));
        }

        public HttpClient CreateClient()
        {
            return _testServer!.CreateClient();
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }

    /// <summary>
    /// Web application factory for HTTP header integration testing
    /// </summary>
    public class HttpHeaderTestApplicationFactory : IDisposable
    {
        private TestServer? _testServer;

        public HttpHeaderTestApplicationFactory()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(HttpHeaderTestController).Assembly);
                    services.AddMediatRCompatibility(typeof(TestGetDataQueryHandler).Assembly);
                })

                .Configure(app =>
                {
                    // Add exception handling middleware first to catch exceptions from header validation
                    app.UseSerializedExceptionHandler();
                    app.UseRouting();
                    // Add header validation middleware for each header type
                    app.UseHttpHeader<HttpAcceptLanguageHeaderAttribute>();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                }));
        }

        public HttpClient CreateClient()
        {
            return _testServer!.CreateClient();
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }

    /// <summary>
    /// Web application factory for test-flag header integration testing
    /// </summary>
    public class TestFlagHeaderTestApplicationFactory : IDisposable
    {
        private TestServer? _testServer;

        public TestFlagHeaderTestApplicationFactory()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(HttpTestFlagHeaderTestController).Assembly);
                    services.AddMediatRCompatibility(typeof(TestGetDataQueryHandler).Assembly);
                })
                .Configure(app =>
                {
                    // Add exception handling middleware first to catch exceptions from header validation
                    app.UseSerializedExceptionHandler();
                    app.UseRouting();
                    // Add header validation middleware for test-flag header type
                    app.UseHttpHeader<HttpTestFlagHeaderAttribute>();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                }));
        }

        public HttpClient CreateClient()
        {
            return _testServer!.CreateClient();
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }

    /// <summary>
    /// Dummy Program class for entry point
    /// </summary>
    public partial class Program { }
}
