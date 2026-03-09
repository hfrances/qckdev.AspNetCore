using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace qckdev.AspNetCore.Test.Fixtures
{
    internal static class MediatRCompatibilityExtensions
    {
        public static IServiceCollection AddMediatRCompatibility(this IServiceCollection services, Assembly assembly)
        {
#if MEDIATR_LEGACY
            services.AddMediatR(assembly);
#else
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
#endif
            return services;
        }
    }
}
