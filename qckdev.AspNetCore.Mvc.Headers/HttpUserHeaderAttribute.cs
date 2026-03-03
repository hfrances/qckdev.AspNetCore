using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Specifies that requests contain the 'sw-user' header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpUserHeaderAttribute : Attribute, IHttpHeaderAttribute
    {

        public string HeaderName { get; } = "sw-user";
        public bool IsAvailable { get; }
        public bool IsMandatory { get; }

        public HttpUserHeaderAttribute()
            : this(true, true) { }

        public HttpUserHeaderAttribute(bool isAvailable)
            : this(isAvailable, true) { }

        public HttpUserHeaderAttribute(bool isAvailable = true, bool isMandatory = true)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }

    }
}
