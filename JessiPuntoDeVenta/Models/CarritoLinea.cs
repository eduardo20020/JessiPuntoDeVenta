namespace JessiPuntoDeVenta.Models;

public sealed class CarritoLinea
{
    public long IdProducto { get; set; }
    public string Nombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
