using JessiPuntoDeVenta.Data.Repositories;
using JessiPuntoDeVenta.Models;
using Microsoft.AspNetCore.Mvc;

namespace JessiPuntoDeVenta.Controllers;

public class ReportesController : Controller
{
    private readonly IReportesRepository _reportes;

    public ReportesController(IReportesRepository reportes)
    {
        _reportes = reportes;
    }

    public async Task<IActionResult> Index(ReportesIndexViewModel? model, CancellationToken cancellationToken)
    {
        model ??= new ReportesIndexViewModel();
        var desde = model.Desde.Date;
        var hastaInclusivo = model.Hasta.Date;
        var hastaExclusivo = hastaInclusivo.AddDays(1);

        var porDia = await _reportes.ResumenPorDiaAsync(desde, hastaExclusivo, cancellationToken);
        var totales = await _reportes.TotalesEnRangoAsync(desde, hastaExclusivo, cancellationToken)
                      ?? new ResumenDiaDto();

        model.PorDia = porDia;
        model.Totales = totales;
        model.Desde = desde;
        model.Hasta = hastaInclusivo;
        return View(model);
    }
}
