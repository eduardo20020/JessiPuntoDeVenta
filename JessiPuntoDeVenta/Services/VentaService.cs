using System.Data;
using Dapper;
using JessiPuntoDeVenta.Data;
using JessiPuntoDeVenta.Data.Repositories;
using MySqlConnector;

namespace JessiPuntoDeVenta.Services;

public sealed class VentaService : IVentaService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IProductoRepository _productos;

    public VentaService(IDbConnectionFactory connectionFactory, IProductoRepository productos)
    {
        _connectionFactory = connectionFactory;
        _productos = productos;
    }

    public async Task<ResultadoVenta> RegistrarVentaAsync(
        IReadOnlyList<LineaVentaInput> lineas,
        string? formaPago,
        CancellationToken cancellationToken = default)
    {
        if (lineas.Count == 0)
            return new ResultadoVenta(false, "La venta no tiene líneas.", null);

        var merged = lineas
            .GroupBy(l => l.IdProducto)
            .Select(g => new LineaVentaInput(g.Key, g.Sum(x => x.Cantidad)))
            .ToList();

        foreach (var l in merged)
        {
            if (l.Cantidad <= 0)
                return new ResultadoVenta(false, "Las cantidades deben ser mayores a cero.", null);
        }

        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        await using var tx = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        try
        {
            var detalle = new List<(long IdProducto, int Cantidad, decimal PrecioUnitario, decimal? CostoUnitario, decimal ImporteLinea)>();
            decimal total = 0;

            foreach (var linea in merged)
            {
                var producto = await _productos.ObtenerPorIdParaVentaAsync(linea.IdProducto, tx, cancellationToken);
                if (producto is null)
                    return await RollbackAsync(tx, "Producto no encontrado.");
                if (!producto.Activo)
                    return await RollbackAsync(tx, $"El producto '{producto.Nombre}' está inactivo.");
                if (producto.Existencias < linea.Cantidad)
                    return await RollbackAsync(tx, $"Stock insuficiente para '{producto.Nombre}'.");

                var importeLinea = decimal.Round(producto.PrecioVenta * linea.Cantidad, 4, MidpointRounding.AwayFromZero);
                total += importeLinea;
                detalle.Add((producto.Id, linea.Cantidad, producto.PrecioVenta, producto.PrecioCosto, importeLinea));
            }

            total = decimal.Round(total, 4, MidpointRounding.AwayFromZero);

            await conn.ExecuteAsync(
                new CommandDefinition(
                    """
                    INSERT INTO ventas (total, forma_pago)
                    VALUES (@total, @formaPago)
                    """,
                    new { total, formaPago },
                    tx,
                    cancellationToken: cancellationToken));

            var ventaId = await conn.ExecuteScalarAsync<long>(
                new CommandDefinition("SELECT LAST_INSERT_ID()", transaction: tx, cancellationToken: cancellationToken));

            foreach (var d in detalle)
            {
                await conn.ExecuteAsync(
                    new CommandDefinition(
                        """
                        INSERT INTO lineas_venta (id_venta, id_producto, cantidad, precio_unitario, costo_unitario, importe_linea)
                        VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @costoUnitario, @importeLinea)
                        """,
                        new
                        {
                            idVenta = ventaId,
                            idProducto = d.IdProducto,
                            d.Cantidad,
                            precioUnitario = d.PrecioUnitario,
                            costoUnitario = d.CostoUnitario,
                            importeLinea = d.ImporteLinea
                        },
                        tx,
                        cancellationToken: cancellationToken));

                var rows = await _productos.DescontarExistenciasAsync(d.IdProducto, d.Cantidad, tx, cancellationToken);
                if (rows != 1)
                    return await RollbackAsync(tx, "No se pudo actualizar el inventario (posible conflicto).");
            }

            await tx.CommitAsync(cancellationToken);
            return new ResultadoVenta(true, null, ventaId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            return new ResultadoVenta(false, ex.Message, null);
        }
    }

    private static async Task<ResultadoVenta> RollbackAsync(MySqlTransaction tx, string error)
    {
        await tx.RollbackAsync();
        return new ResultadoVenta(false, error, null);
    }
}
