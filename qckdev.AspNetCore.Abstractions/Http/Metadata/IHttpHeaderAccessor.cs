namespace qckdev.AspNetCore.Http.Metadata
{
    /// <summary>
    /// Provides methods for accessing HTTP headers in the current request.
    /// </summary>
    public interface IHttpHeaderAccessor
    {

        /// <summary>
        /// Gets the value of an HTTP header by name.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <returns>The header value, or null if not present.</returns>
        string? GetHttpHeader(string name);

        /// <summary>
        /// Gets the value of an HTTP header by type.
        /// </summary>
        /// <typeparam name="THttpHeader">The type implementing <see cref="IHttpHeaderAttribute"/></typeparam>
        /// <returns>The header value, or null if not present.</returns>
        string? GetHttpHeader<THttpHeader>()
            where THttpHeader : class, IHttpHeaderAttribute, new();

    }
}
