using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using qckdev.AspNetCore.Exceptions;
using qckdev.AspNetCore.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Mvc.Controllers
{
    /// <summary>
    /// Base controller for API endpoints with MediatR, logging, and localization support.
    /// </summary>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {

        private IMediator? _mediator;
        private ILogger? _logger;
        private IStringLocalizer<IApplicationResource>? _localizer;

        /// <summary>
        /// Gets the MediatR mediator instance.
        /// </summary>
        protected IMediator Mediator
            => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        /// <summary>
        /// Gets a logger typed to the current controller.
        /// </summary>
        protected ILogger Logger
            => _logger ??= (ILogger)HttpContext.RequestServices.GetRequiredService(
                typeof(ILogger<>).MakeGenericType(this.GetType())
            );

        /// <summary>
        /// Gets the localizer for application resources.
        /// </summary>
        protected IStringLocalizer<IApplicationResource>? Localizer
            => _localizer ??= HttpContext.RequestServices.GetService<IStringLocalizer<IApplicationResource>>();

        /// <summary>
        /// Sends a MediatR request and handles exceptions with localization support.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The MediatR request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the mediator.</returns>
        /// <exception cref="HttpHandledException">Thrown if an API exception occurs and cannot be localized.</exception>
        protected virtual async Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Mediator.Send(request, cancellationToken);
            }
            catch (ApiException ex) when (!string.IsNullOrWhiteSpace(ex.ResourceId))
            {
                var localized = Localizer?.GetString(ex.ResourceId, ex.Parameters ?? Array.Empty<string>());

                if (localized == null || localized.ResourceNotFound)
                {
                    Logger?.LogError(ex, ex.Message);
                    throw;
                }

                Logger?.LogError(localized.Value);
                throw new HttpHandledException(ex.ErrorCode, localized.Value, ex);
            }
            catch (HttpHandledException ex)
            {
                Logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends a MediatR request without response and handles exceptions with localization support.
        /// </summary>
        /// <param name="request">The MediatR request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="HttpHandledException">Thrown if an API exception occurs and cannot be localized.</exception>
        protected virtual async Task Send(
            IRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await Mediator.Send(request, cancellationToken);
            }
            catch (ApiException ex) when (!string.IsNullOrWhiteSpace(ex.ResourceId))
            {
                var localized = Localizer?.GetString(ex.ResourceId, ex.Parameters ?? Array.Empty<string>());

                if (localized == null || localized.ResourceNotFound)
                {
                    Logger?.LogError(ex, ex.Message);
                    throw;
                }

                Logger?.LogError(localized.Value);
                throw new HttpHandledException(ex.ErrorCode, localized.Value, ex);
            }
            catch (HttpHandledException ex)
            {
                Logger?.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
