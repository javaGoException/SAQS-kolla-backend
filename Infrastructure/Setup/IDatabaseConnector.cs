using System.Data;

namespace SAQS_kolla_backend.Infrastructure.Setup;

public interface IDatabaseConnector
{
    Task<IDbConnection> OpenConnectionAsync();
}