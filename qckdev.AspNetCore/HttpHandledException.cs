using System;
using System.Net;

namespace qckdev.AspNetCore
{
    public sealed class HttpHandledException : Exception
    {

        public HttpHandledException(HttpStatusCode errorCode, string message) : this(errorCode, message, null)
        { }

        public HttpHandledException(HttpStatusCode errorCode, string message, dynamic content) : this(errorCode, message, (object)content, null)
        { }

        public HttpHandledException(HttpStatusCode errorCode, string message, dynamic content, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
            this.Content = content;
        }

        public HttpStatusCode ErrorCode { get; }
        public dynamic Content { get; }

    }
}
