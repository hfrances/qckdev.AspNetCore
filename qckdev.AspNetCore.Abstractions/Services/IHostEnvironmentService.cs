namespace qckdev.AspNetCore.Services
{
    /// <summary>
    /// Provides information about the host environment.
    /// </summary>
    public interface IHostEnvironmentService
    {

        /// <summary>
        /// Gets the name of the current environment.
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Checks if the current host environment name is "Development".
        /// </summary>
        /// <returns>True if the environment is Development; otherwise, false.</returns>
        bool IsDevelopment();

        /// <summary>
        /// Checks if the current host environment name is "Docker".
        /// </summary>
        /// <returns>True if the environment is Docker; otherwise, false.</returns>
        bool IsDocker();

        /// <summary>
        /// Checks if the current host environment name is "Staging".
        /// </summary>
        /// <returns>True if the environment is Staging; otherwise, false.</returns>
        bool IsStaging();

        /// <summary>
        /// Checks if the current host environment name is "Production".
        /// </summary>
        /// <returns>True if the environment is Production; otherwise, false.</returns>
        bool IsProduction();

    }
}
