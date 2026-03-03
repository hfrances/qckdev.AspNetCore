using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Specifies that requests contain the 'sw-company' header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCompanyHeaderAttribute : Attribute, IHttpHeaderAttribute
    {

        public string HeaderName { get; } = "sw-company";
        public bool IsAvailable { get; }
        public bool IsMandatory { get; }

        public HttpCompanyHeaderAttribute()
            : this(true, false) { }

        public HttpCompanyHeaderAttribute(bool isAvailable)
            : this(isAvailable, false) { }

        public HttpCompanyHeaderAttribute(bool isAvailable = true, bool isMandatory = false)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }

    }
}
