namespace qckdev.AspNetCore.Http.Metadata
{
    /// <summary>
    /// Defines metadata for HTTP headers that can be validated and accessed.
    /// </summary>
    public interface IHttpHeaderAttribute
    {

        /// <summary>
        /// Gets the name of the HTTP header.
        /// </summary>
        string HeaderName { get; }

        /// <summary>
        /// Gets a value indicating whether the header is available in the request.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether the header is mandatory.
        /// If true and the header is not present, validation will fail.
        /// </summary>
        bool IsMandatory { get; }

    }
}
