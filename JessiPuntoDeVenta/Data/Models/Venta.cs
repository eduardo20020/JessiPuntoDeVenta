namespace JessiPuntoDeVenta.Data.Models;

public sealed class Venta
{
    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public decimal Total { get; set; }
    public string? FormaPago { get; set; }
    public long? IdUsuario { get; set; }
}
