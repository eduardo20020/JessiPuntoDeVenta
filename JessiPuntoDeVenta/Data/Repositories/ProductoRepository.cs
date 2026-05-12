using System.Data;
using Dapper;
using JessiPuntoDeVenta.Data.Models;
using MySqlConnector;

namespace JessiPuntoDeVenta.Data.Repositories;

public sealed class ProductoRepository : IProductoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private const string SelectCols = """
        id AS Id,
        id_proveedor AS IdProveedor,
        nombre AS Nombre,
        codigo_barras AS CodigoBarras,
        precio_venta AS PrecioVenta,
        precio_costo AS PrecioCosto,
        existencias AS Existencias,
        activo AS Activo,
        creado_en AS CreadoEn
        """;

    public ProductoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Producto>> ListarActivosAsync(string? busqueda, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var (pattern, idFiltro) = NormalizarBusqueda(busqueda);
        var rows = await conn.QueryAsync<Producto>(
            new CommandDefinition(
                $"""
                 SELECT {SelectCols}
                 FROM productos
                 WHERE activo = 1
                   AND (
                     (@pattern IS NULL AND @idFiltro IS NULL)
                     OR (@pattern IS NOT NULL AND nombre LIKE CONCAT('%', @pattern, '%'))
                     OR (@pattern IS NOT NULL AND codigo_barras = @pattern)
                     OR (@idFiltro IS NOT NULL AND id = @idFiltro)
                   )
                 ORDER BY nombre
                 """,
                new { pattern, idFiltro },
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<Producto>> ListarTodosAsync(string? busqueda, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var (pattern, idFiltro) = NormalizarBusqueda(busqueda);
        var rows = await conn.QueryAsync<Producto>(
            new CommandDefinition(
                $"""
                 SELECT {SelectCols}
                 FROM productos
                 WHERE (
                     (@pattern IS NULL AND @idFiltro IS NULL)
                     OR (@pattern IS NOT NULL AND nombre LIKE CONCAT('%', @pattern, '%'))
                     OR (@pattern IS NOT NULL AND codigo_barras = @pattern)
                     OR (@idFiltro IS NOT NULL AND id = @idFiltro)
                   )
                 ORDER BY activo DESC, nombre
                 """,
                new { pattern, idFiltro },
                cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<Producto?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Producto>(
            new CommandDefinition(
                $"SELECT {SelectCols} FROM productos WHERE id = @id",
                new { id },
                cancellationToken: cancellationToken));
    }

    public Task<Producto?> ObtenerPorIdParaVentaAsync(long id, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var conn = transaction.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
        return conn.QuerySingleOrDefaultAsync<Producto>(
            new CommandDefinition(
                $"SELECT {SelectCols} FROM productos WHERE id = @id FOR UPDATE",
                new { id },
                transaction,
                cancellationToken: cancellationToken));
    }

    public async Task<long> CrearAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var codigo = string.IsNullOrWhiteSpace(producto.CodigoBarras) ? null : producto.CodigoBarras.Trim();
        await conn.ExecuteAsync(
            new CommandDefinition(
                """
                INSERT INTO productos (id_proveedor, nombre, codigo_barras, precio_venta, precio_costo, existencias, activo)
                VALUES (@IdProveedor, @Nombre, @codigo, @PrecioVenta, @PrecioCosto, @Existencias, @Activo)
                """,
                new
                {
                    producto.IdProveedor,
                    producto.Nombre,
                    codigo,
                    producto.PrecioVenta,
                    producto.PrecioCosto,
                    producto.Existencias,
                    producto.Activo
                },
                cancellationToken: cancellationToken));
        return await conn.ExecuteScalarAsync<long>(
            new CommandDefinition("SELECT LAST_INSERT_ID()", cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        var codigo = string.IsNullOrWhiteSpace(producto.CodigoBarras) ? null : producto.CodigoBarras.Trim();
        await conn.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE productos
                SET id_proveedor = @IdProveedor,
                    nombre = @Nombre,
                    codigo_barras = @codigo,
                    precio_venta = @PrecioVenta,
                    precio_costo = @PrecioCosto,
                    existencias = @Existencias,
                    activo = @Activo
                WHERE id = @Id
                """,
                new
                {
                    producto.Id,
                    producto.IdProveedor,
                    producto.Nombre,
                    codigo,
                    producto.PrecioVenta,
                    producto.PrecioCosto,
                    producto.Existencias,
                    producto.Activo
                },
                cancellationToken: cancellationToken));
    }

    public async Task<int> DescontarExistenciasAsync(long idProducto, int cantidad, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var conn = transaction.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
        return await conn.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE productos
                SET existencias = existencias - @cantidad
                WHERE id = @idProducto AND existencias >= @cantidad
                """,
                new { idProducto, cantidad },
                transaction,
                cancellationToken: cancellationToken));
    }

    private static (string? pattern, long? idFiltro) NormalizarBusqueda(string? busqueda)
    {
        if (string.IsNullOrWhiteSpace(busqueda))
            return (null, null);
        var t = busqueda.Trim();
        if (long.TryParse(t, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var id))
            return (t, id);
        return (t, null);
    }
}
