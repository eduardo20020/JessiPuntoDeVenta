using System.Data;
using Dapper;
using JessiPuntoDeVenta.Data.Models;
using MySqlConnector;

namespace JessiPuntoDeVenta.Data.Repositories;

public sealed class ProveedorRepository : IProveedorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private const string SelectCols = """
        id AS Id,
        nombre AS Nombre,
        telefono AS Telefono,
        notas AS Notas,
        activo AS Activo,
        creado_en AS CreadoEn
        """;

    public ProveedorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Proveedor>> ListarActivosAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var rows = await conn.QueryAsync<Proveedor>(
            new CommandDefinition(
                $"SELECT {SelectCols} FROM proveedores WHERE activo = 1 ORDER BY nombre",
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<Proveedor>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var rows = await conn.QueryAsync<Proveedor>(
            new CommandDefinition(
                $"SELECT {SelectCols} FROM proveedores ORDER BY nombre",
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Proveedor>(
            new CommandDefinition(
                $"SELECT {SelectCols} FROM proveedores WHERE id = @id",
                new { id },
                cancellationToken: cancellationToken));
    }

    public async Task<long> CrearAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        await conn.ExecuteAsync(
            new CommandDefinition(
                """
                INSERT INTO proveedores (nombre, telefono, notas, activo)
                VALUES (@Nombre, @Telefono, @Notas, @Activo)
                """,
                new
                {
                    proveedor.Nombre,
                    proveedor.Telefono,
                    proveedor.Notas,
                    proveedor.Activo
                },
                cancellationToken: cancellationToken));
        return await conn.ExecuteScalarAsync<long>(
            new CommandDefinition("SELECT LAST_INSERT_ID()", cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        await conn.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE proveedores
                SET nombre = @Nombre,
                    telefono = @Telefono,
                    notas = @Notas,
                    activo = @Activo
                WHERE id = @Id
                """,
                new
                {
                    proveedor.Id,
                    proveedor.Nombre,
                    proveedor.Telefono,
                    proveedor.Notas,
                    proveedor.Activo
                },
                cancellationToken: cancellationToken));
    }
}
