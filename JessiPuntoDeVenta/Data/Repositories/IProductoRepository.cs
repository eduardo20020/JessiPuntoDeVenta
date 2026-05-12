using System.Data;
using JessiPuntoDeVenta.Data.Models;

namespace JessiPuntoDeVenta.Data.Repositories;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> ListarActivosAsync(string? busqueda, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> ListarTodosAsync(string? busqueda, CancellationToken cancellationToken = default);
    Task<Producto?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Producto?> ObtenerPorIdParaVentaAsync(long id, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<long> CrearAsync(Producto producto, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default);
    Task<int> DescontarExistenciasAsync(long idProducto, int cantidad, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
