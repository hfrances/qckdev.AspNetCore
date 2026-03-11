using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace qckdev.AspNetCore.FluentValidation
{
    /// <summary>
    /// Adapts a FluentValidation validator to the options validation pipeline.
    /// </summary>
    /// <typeparam name="TOptions">Options type being validated.</typeparam>
    public sealed class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="FluentValidationOptions{TOptions}"/>.
        /// </summary>
        /// <param name="scopeFactory">Scope factory used to resolve validators with their intended lifetime.</param>
        public FluentValidationOptions(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc />
        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("Options instance is null.");
            }

            using var scope = _scopeFactory.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
            var result = validator.Validate(options);
            if (result.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            return ValidateOptionsResult.Fail(result.Errors.Select(error => $"{error.PropertyName}: {error.ErrorMessage}"));
        }
    }
}
