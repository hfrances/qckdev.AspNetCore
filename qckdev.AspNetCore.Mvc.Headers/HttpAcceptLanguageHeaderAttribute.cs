using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Specifies that requests contain the 'Accept-Language' header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpAcceptLanguageHeaderAttribute : Attribute, IHttpHeaderAttribute
    {
        /// <summary>
        /// The HTTP header name value for Accept-Language.
        /// </summary>
        public static readonly string HeaderNameValue = "Accept-Language";

        /// <inheritdoc/>
        public string HeaderName { get; } = HeaderNameValue;
        /// <inheritdoc/>
        public bool IsAvailable { get; }
        /// <inheritdoc/>
        public bool IsMandatory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpAcceptLanguageHeaderAttribute"/> class with default settings.
        /// </summary>
        public HttpAcceptLanguageHeaderAttribute()
            : this(true, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpAcceptLanguageHeaderAttribute"/> class with availability setting.
        /// </summary>
        /// <param name="isAvailable">A value indicating whether the header is available.</param>
        public HttpAcceptLanguageHeaderAttribute(bool isAvailable)
            : this(isAvailable, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpAcceptLanguageHeaderAttribute"/> class with availability and mandatory settings.
        /// </summary>
        /// <param name="isAvailable">A value indicating whether the header is available.</param>
        /// <param name="isMandatory">A value indicating whether the header is mandatory.</param>
        public HttpAcceptLanguageHeaderAttribute(bool isAvailable = true, bool isMandatory = false)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }

    }
}
