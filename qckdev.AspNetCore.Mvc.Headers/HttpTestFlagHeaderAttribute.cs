using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Specifies that requests contain the 'test-flag' header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpTestFlagHeaderAttribute : Attribute, IHttpHeaderAttribute
    {
        public string HeaderName { get; } = "test-flag";
        public bool IsAvailable { get; }
        public bool IsMandatory { get; }

        public HttpTestFlagHeaderAttribute()
            : this(true, true) { }

        public HttpTestFlagHeaderAttribute(bool isAvailable)
            : this(isAvailable, true) { }

        public HttpTestFlagHeaderAttribute(bool isAvailable = true, bool isMandatory = true)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }
    }
}
