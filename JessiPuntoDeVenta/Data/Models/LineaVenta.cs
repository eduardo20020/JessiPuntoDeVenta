namespace JessiPuntoDeVenta.Data.Models;

public sealed class LineaVenta
{
    public long Id { get; set; }
    public long IdVenta { get; set; }
    public long IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? CostoUnitario { get; set; }
    public decimal ImporteLinea { get; set; }
}
