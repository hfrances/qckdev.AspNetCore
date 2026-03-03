using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for localization services.
    /// </summary>
    public static class QLocalizationDependencyInjection
    {

        /// <summary>
        /// Adds localization services for the application with automatic resource discovery.
        /// </summary>
        /// <typeparam name="TApplicationResource">The type implementing <see cref="IApplicationResource"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddLocalization<TApplicationResource>(this IServiceCollection services)
            where TApplicationResource : class, IApplicationResource
        {
            return AddLocalization<TApplicationResource>(services, null);
        }

        /// <summary>
        /// Adds localization services for the application with automatic resource discovery and a default culture.
        /// </summary>
        /// <typeparam name="TApplicationResource">The type implementing <see cref="IApplicationResource"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="defaultCultureName">The default culture name (e.g., "en-US").</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddLocalization<TApplicationResource>(
            this IServiceCollection services,
            string? defaultCultureName)
            where TApplicationResource : class, IApplicationResource
        {
            services.TryAddSingleton<IStringLocalizer<IApplicationResource>>(
                x => x.GetRequiredService<IStringLocalizer<TApplicationResource>>()
            );

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = GetSupportedCultures<TApplicationResource>();

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                
                if (defaultCultureName != null)
                {
                    options.SetDefaultCulture(defaultCultureName);
                }
            });

            return services;
        }

        /// <summary>
        /// Enables request localization middleware with configured localization options.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            return app;
        }

        /// <summary>
        /// Legacy method. Use AddLocalization instead.
        /// </summary>
        [Obsolete("Use AddLocalization instead.", false)]
        public static IServiceCollection AddApplicationLocalization<TApplicationResource>(this IServiceCollection services)
            where TApplicationResource : class, IApplicationResource
        {
            return AddLocalization<TApplicationResource>(services);
        }

        /// <summary>
        /// Legacy method. Use AddLocalization instead.
        /// </summary>
        [Obsolete("Use AddLocalization instead.", false)]
        public static IServiceCollection AddApplicationLocalization<TApplicationResource>(
            this IServiceCollection services,
            string? defaultCultureName)
            where TApplicationResource : class, IApplicationResource
        {
            return AddLocalization<TApplicationResource>(services, defaultCultureName);
        }

        /// <summary>
        /// Legacy method. Use UseLocalization instead.
        /// </summary>
        [Obsolete("Use UseLocalization instead.", false)]
        public static IApplicationBuilder UseApplicationLocalization(this IApplicationBuilder app)
        {
            return UseLocalization(app);
        }

        private static IList<CultureInfo> GetSupportedCultures<T>()
        {
            var supportedCultures = new List<CultureInfo>();
            var rm = new ResourceManager(typeof(T));

            foreach (var ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (rm.GetResourceSet(ci, true, false) != null)
                {
                    supportedCultures.Add(ci);
                }
            }

            return supportedCultures;
        }

    }
}
