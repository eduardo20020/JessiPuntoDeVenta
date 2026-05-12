using System.ComponentModel.DataAnnotations;

namespace JessiPuntoDeVenta.Models;

public sealed class ProveedorEditViewModel
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string Nombre { get; set; } = "";

    [MaxLength(40)]
    public string? Telefono { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }

    public bool Activo { get; set; } = true;
}
