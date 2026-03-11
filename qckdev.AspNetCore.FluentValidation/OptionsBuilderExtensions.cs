using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.FluentValidation
{
    /// <summary>
    /// Extensions for registering FluentValidation-based options validation.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// Registers a FluentValidation-based validator for the current options type.
        /// </summary>
        /// <typeparam name="TOptions">Options type.</typeparam>
        /// <param name="builder">Options builder.</param>
        /// <returns>The same options builder.</returns>
        public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(this OptionsBuilder<TOptions> builder)
            where TOptions : class
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TOptions>, FluentValidationOptions<TOptions>>());

            return builder;
        }

        /// <summary>
        /// Validates options during startup across all supported target frameworks.
        /// </summary>
        /// <typeparam name="TOptions">Options type.</typeparam>
        /// <param name="builder">Options builder.</param>
        /// <returns>The same options builder.</returns>
        public static OptionsBuilder<TOptions> ValidateOnStartCompat<TOptions>(this OptionsBuilder<TOptions> builder)
            where TOptions : class
        {
#if NET7_0_OR_GREATER
            return builder.ValidateOnStart();
#else
            builder.Services.AddSingleton<IHostedService>(serviceProvider =>
                new OptionsValidateOnStartHostedService<TOptions>(
                    serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>(),
                    builder.Name));

            return builder;
#endif
        }

#if !NET7_0_OR_GREATER
        private sealed class OptionsValidateOnStartHostedService<TOptions> : IHostedService
            where TOptions : class
        {
            private readonly IOptionsMonitor<TOptions> _monitor;
            private readonly string? _name;

            public OptionsValidateOnStartHostedService(IOptionsMonitor<TOptions> monitor, string? name)
            {
                _monitor = monitor;
                _name = name;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(_name) || _name == Options.DefaultName)
                {
                    _ = _monitor.CurrentValue;
                }
                else
                {
                    _ = _monitor.Get(_name);
                }

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
                => Task.CompletedTask;
        }
#endif
    }
}
