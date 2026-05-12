using Dapper;
using MySqlConnector;

namespace JessiPuntoDeVenta.Data;

public interface IDatabaseProbe
{
    Task<DatabaseProbeResult> CheckAsync(CancellationToken cancellationToken = default);
}

public sealed record DatabaseProbeResult(bool Ok, string Message, long? ProductoCount);

public sealed class DatabaseProbe : IDatabaseProbe
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseProbe(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DatabaseProbeResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync(cancellationToken);
            await conn.ExecuteScalarAsync<int>(new CommandDefinition("SELECT 1", cancellationToken: cancellationToken));
            var count = await conn.ExecuteScalarAsync<long?>(
                new CommandDefinition("SELECT COUNT(*) FROM productos", cancellationToken: cancellationToken));
            return new DatabaseProbeResult(true, "Conexión correcta.", count);
        }
        catch (Exception ex)
        {
            return new DatabaseProbeResult(false, ex.Message, null);
        }
    }
}
