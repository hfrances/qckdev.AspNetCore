using System;
using System.Net;
using System.Runtime.Serialization;

namespace qckdev.AspNetCore.Exceptions
{
    /// <summary>
    /// A specialized exception for API operations with localization support.
    /// Extends <see cref="HttpHandledException"/> with resource-based messages for multilingual support.
    /// </summary>
    [Serializable]
    public class ApiException : HttpHandledException
    {

        /// <summary>
        /// Gets the resource identifier for message localization.
        /// </summary>
        public string? ResourceId { get; }

        /// <summary>
        /// Gets or sets the parameters for the localized message.
        /// </summary>
        public virtual object[]? Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with an error code and resource identifier.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the exception.</param>
        /// <param name="resourceId">The resource identifier for message localization.</param>
        public ApiException(HttpStatusCode statusCode, string? resourceId)
            : this(statusCode, resourceId, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with an error code, resource identifier, and inner exception.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the exception.</param>
        /// <param name="resourceId">The resource identifier for message localization.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public ApiException(HttpStatusCode statusCode, string? resourceId, Exception? innerException)
            : base(statusCode, resourceId, innerException)
        {
            ResourceId = resourceId;
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
        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
