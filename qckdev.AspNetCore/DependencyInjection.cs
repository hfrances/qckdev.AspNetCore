using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using qckdev.AspNetCore.Middleware;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Extension methods for services in an <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
    /// </summary>
    public static class QDependencyInjection
    {

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
