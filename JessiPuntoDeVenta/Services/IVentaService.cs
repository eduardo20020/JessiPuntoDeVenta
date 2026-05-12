namespace JessiPuntoDeVenta.Services;

public sealed record LineaVentaInput(long IdProducto, int Cantidad);

public sealed record ResultadoVenta(bool Ok, string? Error, long? IdVenta);

public interface IVentaService
{
    Task<ResultadoVenta> RegistrarVentaAsync(
        IReadOnlyList<LineaVentaInput> lineas,
        string? formaPago,
        CancellationToken cancellationToken = default);
}
