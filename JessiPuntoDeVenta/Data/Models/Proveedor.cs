namespace JessiPuntoDeVenta.Data.Models;

public sealed class Proveedor
{
    public long Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Telefono { get; set; }
    public string? Notas { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; }
}
