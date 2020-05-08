using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace OrderManager.Infrastructure
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection CreateConnection();
        Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
       
    }
}