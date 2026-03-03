using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using qckdev.AspNetCore.Exceptions;
using qckdev.AspNetCore.Http.Metadata;
using System;
using System.Net;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Middleware that validates required HTTP headers in the request.
    /// </summary>
    /// <typeparam name="THttpHeaderAttribute">The type of header attribute to validate.</typeparam>
    sealed class HttpHeaderValidatorMiddleware<THttpHeaderAttribute>
        where THttpHeaderAttribute : class, IHttpHeaderAttribute, new()
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<HttpHeaderValidatorMiddleware<THttpHeaderAttribute>> _logger;
        private readonly string _headerName;

        public HttpHeaderValidatorMiddleware(
            RequestDelegate next,
            ILogger<HttpHeaderValidatorMiddleware<THttpHeaderAttribute>> logger,
            string headerName)
        {
            _next = next;
            _logger = logger;
            _headerName = headerName;
        }

        public async Task Invoke(HttpContext context)
        {
            var headerAttribute = new THttpHeaderAttribute();

            if (headerAttribute.IsMandatory)
            {
                var headerValue = context.Request.Headers[_headerName];

                if (string.IsNullOrWhiteSpace(headerValue))
                {
                    _logger.LogWarning($"Mandatory header '{_headerName}' is missing from request");
                    throw new HttpHandledException(
                        HttpStatusCode.BadRequest,
                        $"Required header '{_headerName}' is missing"
                    );
                }
            }

            await _next(context);
        }

    }
}
