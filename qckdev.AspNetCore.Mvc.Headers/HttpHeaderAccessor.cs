using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Default implementation of <see cref="IHttpHeaderAccessor"/>.
    /// </summary>
    sealed class HttpHeaderAccessor : IHttpHeaderAccessor
    {

        public IHttpContextAccessor HttpContextAccessor { get; }

        public HttpHeaderAccessor(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public string? GetHttpHeader(string name)
            => HttpContextAccessor.HttpContext?.Request.Headers[name];

        /// <inheritdoc/>
        public string? GetHttpHeader<THttpHeader>()
            where THttpHeader : class, IHttpHeaderAttribute, new()
            => GetHttpHeader(Activator.CreateInstance<THttpHeader>().HeaderName);

    }
}
