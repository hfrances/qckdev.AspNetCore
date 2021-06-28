using Microsoft.AspNetCore.Builder;
using qckdev.AspNetCore.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QInfrastructureDependencyInjection
    {

        public static IServiceCollection AddDataInitializer<TDataInitializer>(this IServiceCollection services)
            where TDataInitializer : class, IDataInitializer
        {
            return services.AddScoped<IDataInitializer, TDataInitializer>();
        }

        public static void DataInitialization(this IApplicationBuilder app)
        {

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var tasks = new List<Task>();
                var dataInitializers = scope
                    .ServiceProvider
                           .GetServices<IDataInitializer>();

                foreach (var initializer in dataInitializers)
                {
                    tasks.Add(
                        initializer.InitializeAsync(CancellationToken.None)
                    );
                }
                Task.WhenAll(tasks);
            }
        }

    }
}
