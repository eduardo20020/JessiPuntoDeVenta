using JessiPuntoDeVenta.Data.Models;

namespace JessiPuntoDeVenta.Data.Repositories;

public interface IProveedorRepository
{
    Task<IReadOnlyList<Proveedor>> ListarActivosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Proveedor>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<Proveedor?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> CrearAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
}
