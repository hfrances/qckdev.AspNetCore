using Microsoft.AspNetCore.Builder;
using qckdev.AspNetCore.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// <see cref="IServiceCollection"/> and <see cref="IApplicationBuilder"/> extension methods for the persistence layer.
    /// </summary>
    public static class QPersistenceDependencyInjection
    {

        /// <summary>
        /// Adds a scoped service of <see cref="IDataInitializer"/> type for perform some operations on <see cref="UseDataInitializer(IApplicationBuilder)"/> call.
        /// </summary>
        /// <typeparam name="TDataInitializer">The type of the <see cref="IDataInitializer"/> service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddDataInitializer<TDataInitializer>(this IServiceCollection services)
            where TDataInitializer : class, IDataInitializer
        {
            return services.AddScoped<IDataInitializer, TDataInitializer>();
        }

        /// <summary>
        /// Performs actions registered with <see cref="AddDataInitializer{TDataInitializer}(IServiceCollection)"/> method.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A reference to the app after the operation has completed.</returns>
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder app)
        {

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var tasks = new List<Task>();
                var cancellationToken = default(CancellationToken);
                var dataInitializers = scope
                    .ServiceProvider
                           .GetServices<IDataInitializer>();

                foreach (var initializer in dataInitializers)
                {
                    tasks.Add(
                        initializer.InitializeAsync(cancellationToken)
                    );
                }
                Task.WhenAll(tasks).Wait();
            }
            return app;
        }

    }
}
