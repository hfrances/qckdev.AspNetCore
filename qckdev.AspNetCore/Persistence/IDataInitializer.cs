using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Persistence
{
    public interface IDataInitializer
    {

        Task InitializeAsync(CancellationToken cancellationToken);

    }
}
