using System.ComponentModel.DataAnnotations;
using JessiPuntoDeVenta.Data.Repositories;

namespace JessiPuntoDeVenta.Models;

public sealed class ReportesIndexViewModel
{
    [Display(Name = "Desde")]
    [DataType(DataType.Date)]
    public DateTime Desde { get; set; } = DateTime.Today.AddDays(-30);

    [Display(Name = "Hasta")]
    [DataType(DataType.Date)]
    public DateTime Hasta { get; set; } = DateTime.Today;

    public IReadOnlyList<ResumenDiaDto> PorDia { get; set; } = Array.Empty<ResumenDiaDto>();
    public ResumenDiaDto Totales { get; set; } = new();
}
