using System.ComponentModel.DataAnnotations;

namespace JessiPuntoDeVenta.Models;

public sealed class ProductoEditViewModel
{
    public long? Id { get; set; }

    public long? IdProveedor { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(300)]
    public string Nombre { get; set; } = "";

    [MaxLength(64)]
    public string? CodigoBarras { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal PrecioVenta { get; set; }

    [Range(typeof(decimal), "0", "999999999999")]
    public decimal? PrecioCosto { get; set; }

    [Range(0, int.MaxValue)]
    public int Existencias { get; set; }

    public bool Activo { get; set; } = true;
}
