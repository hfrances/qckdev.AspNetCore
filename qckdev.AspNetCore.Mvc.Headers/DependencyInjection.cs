using Microsoft.AspNetCore.Builder;
using qckdev.AspNetCore.Http.Metadata;
using qckdev.AspNetCore.Mvc.Headers;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for HTTP header services.
    /// </summary>
    public static class QMvcHeadersDependencyInjection
    {

        /// <summary>
        /// Adds the HTTP header accessor service for accessing headers in the request.
        /// Requires HttpContextAccessor to be available.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddHttpHeaderAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IHttpHeaderAccessor, HttpHeaderAccessor>();
            return services;
        }

        /// <summary>
        /// Adds a middleware to validate HTTP headers for a specific header attribute type.
        /// </summary>
        /// <typeparam name="THttpHeaderAttribute">The type of header attribute to validate.</typeparam>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseHttpHeader<THttpHeaderAttribute>(this IApplicationBuilder app)
            where THttpHeaderAttribute : class, IHttpHeaderAttribute, new()
        {
            var header = Activator.CreateInstance<THttpHeaderAttribute>();
            app.UseMiddleware<HttpHeaderValidatorMiddleware<THttpHeaderAttribute>>(header.HeaderName);
            return app;
        }

    }
}
