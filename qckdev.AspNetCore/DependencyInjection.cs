using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using qckdev.AspNetCore.Middleware;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QDependencyInjection
    {

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Singleton"/> 
        /// service with the implementationType implementation to the collection if the service
        /// type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            collection.TryAddSingleton(typeof(TService), typeof(TImplementation));
            return collection;
        }

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Singleton"/> 
        /// service with the implementationType implementation to the collection if the service
        /// type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddSingleton<TService>(this IServiceCollection collection)
            where TService : class
        {
            collection.TryAddSingleton(typeof(TService));
            return collection;
        }

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Scoped"/> service to the collection if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddScoped(typeof(TService), typeof(TImplementation));
            return services;
        }

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Scoped"/> service to the collection if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddScoped<TService>(this IServiceCollection services)
            where TService : class
        {
            services.TryAddScoped(typeof(TService));
            return services;
        }

        /// <summary>
        /// Adds a middleware type to the application's request pipeline for parsing <see cref="Exception"/> classes to json.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseExceptionHandlerResponse(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerResponseMiddleware>();

            return app;
        }

    }
}
