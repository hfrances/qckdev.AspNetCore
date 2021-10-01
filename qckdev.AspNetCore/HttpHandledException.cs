using System;
using System.Net;
using System.Runtime.Serialization;

namespace qckdev.AspNetCore
{

    [Serializable]
    public class HttpHandledException : Exception
    {

        public HttpHandledException(HttpStatusCode errorCode, string message) : this(errorCode, message, null)
        { }

        public HttpHandledException(HttpStatusCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        protected HttpHandledException(SerializationInfo info, StreamingContext context) : base(info, context) { }


        public HttpStatusCode ErrorCode { get; }
        public virtual dynamic Content { get; }

    }
}
