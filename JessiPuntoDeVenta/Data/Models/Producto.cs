namespace JessiPuntoDeVenta.Data.Models;

public sealed class Producto
{
    public long Id { get; set; }
    public long? IdProveedor { get; set; }
    public string Nombre { get; set; } = "";
    public string? CodigoBarras { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal? PrecioCosto { get; set; }
    public int Existencias { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; }
}
