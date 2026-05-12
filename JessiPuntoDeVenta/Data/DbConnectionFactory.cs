using System.Data;
using MySqlConnector;

namespace JessiPuntoDeVenta.Data;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'ConnectionStrings:Default' is not configured.");
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}
