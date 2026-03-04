using Microsoft.AspNetCore.Http;
using qckdev.AspNetCore.Http.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Middleware that validates required HTTP headers in the request.
    /// </summary>
    /// <typeparam name="THttpHeaderAttribute">The type of header attribute to validate.</typeparam>
    sealed class HttpHeaderValidatorMiddleware<THttpHeaderAttribute>
        where THttpHeaderAttribute : class, IHttpHeaderAttribute
    {
        private readonly RequestDelegate _next;
        private readonly string _headerName;

        public HttpHeaderValidatorMiddleware(RequestDelegate next, string headerName)
        {
            _next = next;
            _headerName = headerName;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsHeaderValidated(context))
            {
                await _next.Invoke(context);
            }
            else
            {
                throw new Exception($"The header '{_headerName}' is mandatory and it is missing");
            }
        }

        private bool IsHeaderValidated(HttpContext context)
        {
            Endpoint? endpoint = context.GetEndpoint();

            if (endpoint == null)
                return true;

            bool isRequired = IsHeaderAvailable(endpoint);
            if (!isRequired)
                return true;

            bool isIncluded = IsHeaderIncluded(endpoint, context);

            if (isRequired && isIncluded)
                return true;

            return false;
        }

        private bool IsHeaderIncluded(Endpoint endpoint, HttpContext context)
        {
            var attribute = endpoint.Metadata.GetMetadata<THttpHeaderAttribute>();
            
            if (attribute is {IsMandatory: false })
                return true;

            // Check if header exists and has a non-empty value
            var headerKey = context.Request.Headers.Keys
                .FirstOrDefault(k => k.Equals(_headerName, StringComparison.OrdinalIgnoreCase));

            if (headerKey == null)
                return false;

            // Validate that the header value is not null or empty (but whitespace is allowed)
            var headerValue = context.Request.Headers[headerKey].ToString();
            return !string.IsNullOrEmpty(headerValue);
        }

        private bool IsHeaderAvailable(Endpoint endpoint)
        {
            var attribute = endpoint.Metadata.GetMetadata<THttpHeaderAttribute>();

            return attribute is { IsAvailable: true };
        }
    }
}
