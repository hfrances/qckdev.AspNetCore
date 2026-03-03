namespace qckdev.AspNetCore.Localization
{
    /// <summary>
    /// Marker interface for application resource types used in localization.
    /// Implement this interface in your resource type and use it with <see cref="Microsoft.Extensions.Localization.IStringLocalizer{TResource}"/>.
    /// </summary>
    /// <remarks>
    /// This interface is for backward compatibility. Consider moving to the localized version in qckdev.AspNetCore.Localization namespace.
    /// </remarks>
    [System.Obsolete("Use the interface from qckdev.AspNetCore.Localization namespace instead.", false)]
    public interface IApplicationResource
    {
    }
}
