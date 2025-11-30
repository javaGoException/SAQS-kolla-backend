using System.Data.Common;

namespace Infrastructure.Setup;

public interface IDatabaseConnector
{
    Task<DbConnection> OpenConnectionAsync();
}