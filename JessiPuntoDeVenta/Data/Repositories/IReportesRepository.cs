namespace JessiPuntoDeVenta.Data.Repositories;

public sealed class ResumenDiaDto
{
    public DateTime Dia { get; set; }
    public decimal VentasTotales { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal MargenBruto { get; set; }
}

public interface IReportesRepository
{
    Task<IReadOnlyList<ResumenDiaDto>> ResumenPorDiaAsync(DateTime desdeInicioDia, DateTime hastaExclusivo, CancellationToken cancellationToken = default);
    Task<ResumenDiaDto?> TotalesEnRangoAsync(DateTime desdeInicioDia, DateTime hastaExclusivo, CancellationToken cancellationToken = default);
}
