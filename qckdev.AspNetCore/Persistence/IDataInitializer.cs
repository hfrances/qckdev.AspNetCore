using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Persistence
{

    /// <summary>
    /// Provides automation for data initialization using <see cref="QPersistenceDependencyInjection.AddDataInitializer{TDataInitializer}(IServiceCollection)"/>
    /// </summary>
    public interface IDataInitializer
    {

        /// <summary>
        /// Performs data initialization.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns></returns>
        Task InitializeAsync(CancellationToken cancellationToken);

    }
}
