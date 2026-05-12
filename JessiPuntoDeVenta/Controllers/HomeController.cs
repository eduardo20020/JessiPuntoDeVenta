using System.Diagnostics;
using JessiPuntoDeVenta.Data;
using JessiPuntoDeVenta.Models;
using Microsoft.AspNetCore.Mvc;

namespace JessiPuntoDeVenta.Controllers;

public class HomeController : Controller
{
    private readonly IDatabaseProbe _databaseProbe;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IDatabaseProbe databaseProbe, ILogger<HomeController> logger)
    {
        _databaseProbe = databaseProbe;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var db = await _databaseProbe.CheckAsync(cancellationToken);
        return View(db);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
