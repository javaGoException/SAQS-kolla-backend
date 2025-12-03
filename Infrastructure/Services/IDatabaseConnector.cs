using System.Data;

namespace SAQS_kolla_backend.Infrastructure.Services;

public interface IDatabaseConnector
{
    Task<IDbConnection> OpenConnectionAsync();
}