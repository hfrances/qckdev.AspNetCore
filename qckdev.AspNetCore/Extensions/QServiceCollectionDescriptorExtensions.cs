using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace qckdev.AspNetCore.Extensions
{

    /// <summary>
    /// Extension methods for adding and removing services to an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class QServiceCollectionDescriptorExtensions
    {

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Singleton"/> 
        /// service with the implementationType implementation to the collection if the service
        /// type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddSingleton(typeof(TService), typeof(TImplementation));
            return services;
        }

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Singleton"/> 
        /// service with the implementationType implementation to the collection if the service
        /// type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddSingleton<TService>(this IServiceCollection services)
            where TService : class
        {
            services.TryAddSingleton(typeof(TService));
            return services;
        }

        /// <summary>
        /// Adds the specified service as a <see cref="ServiceLifetime.Scoped"/> service to the collection if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
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
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection TryAddScoped<TService>(this IServiceCollection services)
            where TService : class
        {
            services.TryAddScoped(typeof(TService));
            return services;
        }

    }
}
