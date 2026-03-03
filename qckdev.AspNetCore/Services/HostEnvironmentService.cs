using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using qckdev.AspNetCore.Services;

namespace qckdev.AspNetCore
{
    /// <summary>
    /// Default implementation of <see cref="IHostEnvironmentService"/>.
    /// </summary>
    sealed class HostEnvironmentService : IHostEnvironmentService
    {

        static readonly string EnvironmentDocker = "Docker";

        public IWebHostEnvironment Environment { get; }

        public string EnvironmentName => Environment.EnvironmentName;

        public HostEnvironmentService(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        /// <inheritdoc/>
        public bool IsDevelopment()
            => Environment.IsDevelopment();

        /// <inheritdoc/>
        public bool IsDocker()
            => Environment.IsEnvironment(EnvironmentDocker);

        /// <inheritdoc/>
        public bool IsStaging()
            => Environment.IsStaging();

        /// <inheritdoc/>
        public bool IsProduction()
            => Environment.IsProduction();

    }
}
