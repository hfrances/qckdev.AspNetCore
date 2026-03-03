using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using qckdev.AspNetCore;
using qckdev.AspNetCore.Middlewares;
using qckdev.AspNetCore.Services;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Extension methods for services in an <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
    /// </summary>
    public static class QDependencyInjection
    {

        const string BASE_PATH_KEY = "BasePath";

        /// <summary>
        /// Adds a default implementation for the <see cref="IHostEnvironmentService"/> service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddHostEnvironmentService(this IServiceCollection services)
        {
            services.TryAddSingleton<IHostEnvironmentService, HostEnvironmentService>();
            return services;
        }

        /// <summary>
        /// Loads the value from "BasePath" property set in appsettings file. It may have multiple values split by ";" character.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UsePathBase(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var basePathRaw = (configuration.GetSection(BASE_PATH_KEY)?.Value ?? "/");
            var basePaths = basePathRaw.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

            foreach (var basePath in basePaths)
            {
                app.UsePathBase(basePath);
            }
            return app;
        }

        /// <summary>
        /// Adds a middleware type to the application's request pipeline for parsing <see cref="Exception"/> classes to json.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseSerializedExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<SerializedExceptionHandlerResponseMiddleware>();

            return app;
        }

    }
}
