using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace qckdev.AspNetCore
{

    /// <summary>
    /// A base class for exceptions handled by <see cref="QDependencyInjection.UseSerializedExceptionHandler(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>.
    /// </summary>
    [Serializable]
    public class HttpHandledException : Exception
    {

        /// <summary>
        /// Gets the error code of the HTTP response.
        /// </summary>
        public HttpStatusCode ErrorCode { get; }

        /// <summary>
        /// Gets or sets additional object information for the exception.
        /// </summary>
        public virtual dynamic Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHandledException"/> class with a specific message that describes the current exception.
        /// </summary>
        public HttpHandledException(HttpStatusCode errorCode, string message) : this(errorCode, message, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHandledException"/> class with a specific message that describes the current exception and an inner exception.
        /// </summary>
        public HttpHandledException(HttpStatusCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        ///  Initializes a new instance of the System.Exception class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected HttpHandledException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
