using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace qckdev.AspNetCore.Test.Fixtures
{
    public class TestApplicationFactory : IDisposable
    {
        private readonly IHost _host;
        private readonly TestServer _testServer;

        public TestApplicationFactory()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer();
                    webBuilder.ConfigureServices(services =>
                    {
                        var assembly = typeof(TestApiController).Assembly;
                        services.AddHttpContextAccessor();
                        services.AddControllers()
                            .AddApplicationPart(assembly);
                        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
                    });
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    });
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

    public partial class Program { }
}
