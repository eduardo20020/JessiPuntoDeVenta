using Microsoft.AspNetCore.Mvc.Rendering;

namespace JessiPuntoDeVenta.Models;

public sealed class VentaIndexViewModel
{
    public IReadOnlyList<CarritoLinea> Lineas { get; set; } = Array.Empty<CarritoLinea>();
    public SelectList? Productos { get; set; }
    public long? IdProductoSeleccionado { get; set; }
    public int Cantidad { get; set; } = 1;
    public string? FormaPago { get; set; }
    public string? Mensaje { get; set; }
    public bool MensajeEsError { get; set; }
    public decimal Total => Lineas.Sum(l => l.Subtotal);
}
