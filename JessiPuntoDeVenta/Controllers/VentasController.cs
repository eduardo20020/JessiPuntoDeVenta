using System.Text.Json;
using JessiPuntoDeVenta.Data.Repositories;
using JessiPuntoDeVenta.Models;
using JessiPuntoDeVenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JessiPuntoDeVenta.Controllers;

public class VentasController : Controller
{
    private const string CartKey = "pos_carrito_v1";
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IProductoRepository _productos;
    private readonly IVentaService _ventaService;

    public VentasController(IProductoRepository productos, IVentaService ventaService)
    {
        _productos = productos;
        _ventaService = ventaService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = await BuildViewModelAsync(cancellationToken);
        if (TempData.TryGetValue("Message", out var msg) && msg is string s)
        {
            vm.Mensaje = s;
            vm.MensajeEsError = TempData["MessageError"] as bool? == true;
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarLinea(long idProducto, int cantidad, CancellationToken cancellationToken)
    {
        if (idProducto <= 0 || cantidad <= 0)
        {
            TempData["Message"] = "Selecciona un producto y una cantidad válida.";
            TempData["MessageError"] = true;
            return RedirectToAction(nameof(Index));
        }

        var producto = await _productos.ObtenerPorIdAsync(idProducto, cancellationToken);
        if (producto is null || !producto.Activo)
        {
            TempData["Message"] = "Producto no disponible.";
            TempData["MessageError"] = true;
            return RedirectToAction(nameof(Index));
        }

        var cart = ReadCart();
        var existing = cart.FirstOrDefault(l => l.IdProducto == idProducto);
        if (existing is not null)
        {
            existing.Cantidad += cantidad;
            existing.Subtotal = decimal.Round(existing.PrecioUnitario * existing.Cantidad, 4, MidpointRounding.AwayFromZero);
        }
        else
        {
            cart.Add(
                new CarritoLinea
                {
                    IdProducto = producto.Id,
                    Nombre = producto.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.PrecioVenta,
                    Subtotal = decimal.Round(producto.PrecioVenta * cantidad, 4, MidpointRounding.AwayFromZero)
                });
        }

        WriteCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuitarLinea(int index)
    {
        var cart = ReadCart();
        if (index >= 0 && index < cart.Count)
            cart.RemoveAt(index);
        WriteCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Vaciar()
    {
        WriteCart(new List<CarritoLinea>());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(string? formaPago, CancellationToken cancellationToken)
    {
        var cart = ReadCart();
        if (cart.Count == 0)
        {
            TempData["Message"] = "El carrito está vacío.";
            TempData["MessageError"] = true;
            return RedirectToAction(nameof(Index));
        }

        var lineas = cart.Select(l => new LineaVentaInput(l.IdProducto, l.Cantidad)).ToList();
        var resultado = await _ventaService.RegistrarVentaAsync(lineas, string.IsNullOrWhiteSpace(formaPago) ? null : formaPago.Trim(), cancellationToken);
        if (!resultado.Ok)
        {
            TempData["Message"] = resultado.Error ?? "No se pudo registrar la venta.";
            TempData["MessageError"] = true;
            return RedirectToAction(nameof(Index));
        }

        WriteCart(new List<CarritoLinea>());
        TempData["Message"] = $"Venta registrada (folio {resultado.IdVenta}).";
        TempData["MessageError"] = false;
        return RedirectToAction(nameof(Index));
    }

    private List<CarritoLinea> ReadCart()
    {
        var json = HttpContext.Session.GetString(CartKey);
        return string.IsNullOrEmpty(json)
            ? new List<CarritoLinea>()
            : JsonSerializer.Deserialize<List<CarritoLinea>>(json, JsonOpts) ?? new List<CarritoLinea>();
    }

    private void WriteCart(List<CarritoLinea> cart) =>
        HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart, JsonOpts));

    private async Task<VentaIndexViewModel> BuildViewModelAsync(CancellationToken cancellationToken)
    {
        var activos = await _productos.ListarActivosAsync(null, cancellationToken);
        return new VentaIndexViewModel
        {
            Lineas = ReadCart(),
            Productos = new SelectList(activos, "Id", "Nombre"),
            Cantidad = 1
        };
    }
}
