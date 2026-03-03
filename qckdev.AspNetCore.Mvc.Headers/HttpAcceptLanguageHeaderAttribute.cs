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

        public string HeaderName { get; } = "Accept-Language";
        public bool IsAvailable { get; }
        public bool IsMandatory { get; }

        public HttpAcceptLanguageHeaderAttribute()
            : this(true, false) { }

        public HttpAcceptLanguageHeaderAttribute(bool isAvailable)
            : this(isAvailable, false) { }

        public HttpAcceptLanguageHeaderAttribute(bool isAvailable = true, bool isMandatory = false)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }

    }
}
