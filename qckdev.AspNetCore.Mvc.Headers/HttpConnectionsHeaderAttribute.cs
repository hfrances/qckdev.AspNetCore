using qckdev.AspNetCore.Http.Metadata;
using System;

namespace qckdev.AspNetCore.Mvc.Headers
{
    /// <summary>
    /// Specifies that requests contain the 'Connection' header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpConnectionsHeaderAttribute : Attribute, IHttpHeaderAttribute
    {

        public string HeaderName { get; } = "Connection";
        public bool IsAvailable { get; }
        public bool IsMandatory { get; }

        public HttpConnectionsHeaderAttribute()
            : this(true, false) { }

        public HttpConnectionsHeaderAttribute(bool isAvailable)
            : this(isAvailable, false) { }

        public HttpConnectionsHeaderAttribute(bool isAvailable = true, bool isMandatory = false)
        {
            IsAvailable = isAvailable;
            IsMandatory = isMandatory;
        }

    }
}
