using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using qckdev.AspNetCore.Mvc.Headers;
using MediatR;

namespace qckdev.AspNetCore.Test.Fixtures
{
    /// <summary>
    /// Web application factory for integration testing
    /// </summary>
    public class TestApplicationFactory : BaseTestApplicationFactory
    {
        public TestApplicationFactory()
            : base(
                services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(TestApiController).Assembly);
                    services.AddMediatR(typeof(TestGetDataQueryHandler).Assembly);
                },
                app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                })
        {
        }
    }

    /// <summary>
    /// Web application factory for HTTP header integration testing
    /// </summary>
    public class HttpHeaderTestApplicationFactory : BaseTestApplicationFactory
    {
        public HttpHeaderTestApplicationFactory()
            : base(
                services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(HttpHeaderTestController).Assembly);
                    services.AddMediatR(typeof(TestGetDataQueryHandler).Assembly);
                },
                app =>
                {
                    // Add exception handling middleware first to catch exceptions from header validation
                    app.UseSerializedExceptionHandler();
                    app.UseRouting();
                    // Add header validation middleware for each header type
                    app.UseHttpHeader<HttpAcceptLanguageHeaderAttribute>();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                })
        {
        }
    }

    /// <summary>
    /// Web application factory for test-flag header integration testing
    /// </summary>
    public class TestFlagHeaderTestApplicationFactory : BaseTestApplicationFactory
    {
        public TestFlagHeaderTestApplicationFactory()
            : base(
                services =>
                {
                    services.AddHttpContextAccessor();
                    services.AddControllers()
                        .AddApplicationPart(typeof(HttpTestFlagHeaderTestController).Assembly);
                    services.AddMediatR(typeof(TestGetDataQueryHandler).Assembly);
                },
                app =>
                {
                    // Add exception handling middleware first to catch exceptions from header validation
                    app.UseSerializedExceptionHandler();
                    app.UseRouting();
                    // Add header validation middleware for test-flag header type
                    app.UseHttpHeader<HttpTestFlagHeaderAttribute>();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                })
        {
        }
    }

    public abstract class BaseTestApplicationFactory : IDisposable
    {
        private readonly IHost _host;
        private readonly TestServer _testServer;

        protected BaseTestApplicationFactory(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configureApp)
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer();
                    webBuilder.ConfigureServices(configureServices);
                    webBuilder.Configure(configureApp);
                })
                .Start();

            _testServer = _host.GetTestServer();
        }

        public HttpClient CreateClient()
        {
            return _testServer.CreateClient();
        }

        public void Dispose()
        {
            _testServer.Dispose();
            _host.Dispose();
        }
    }

    /// <summary>
    /// Dummy Program class for entry point
    /// </summary>
    public partial class Program { }
}
