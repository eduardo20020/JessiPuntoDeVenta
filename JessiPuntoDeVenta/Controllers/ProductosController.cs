using JessiPuntoDeVenta.Data.Models;
using JessiPuntoDeVenta.Data.Repositories;
using JessiPuntoDeVenta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JessiPuntoDeVenta.Controllers;

public class ProductosController : Controller
{
    private readonly IProductoRepository _productos;
    private readonly IProveedorRepository _proveedores;

    public ProductosController(IProductoRepository productos, IProveedorRepository proveedores)
    {
        _productos = productos;
        _proveedores = proveedores;
    }

    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewBag.Q = q;
        var list = await _productos.ListarTodosAsync(q, cancellationToken);
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await CargarProveedoresAsync(cancellationToken);
        return View(new ProductoEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarProveedoresAsync(cancellationToken);
            return View(model);
        }

        await _productos.CrearAsync(
            new Producto
            {
                IdProveedor = model.IdProveedor,
                Nombre = model.Nombre.Trim(),
                CodigoBarras = model.CodigoBarras,
                PrecioVenta = model.PrecioVenta,
                PrecioCosto = model.PrecioCosto,
                Existencias = model.Existencias,
                Activo = model.Activo
            },
            cancellationToken);

        TempData["Message"] = "Producto creado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var p = await _productos.ObtenerPorIdAsync(id, cancellationToken);
        if (p is null)
            return NotFound();

        await CargarProveedoresAsync(cancellationToken);
        return View(
            new ProductoEditViewModel
            {
                Id = p.Id,
                IdProveedor = p.IdProveedor,
                Nombre = p.Nombre,
                CodigoBarras = p.CodigoBarras,
                PrecioVenta = p.PrecioVenta,
                PrecioCosto = p.PrecioCosto,
                Existencias = p.Existencias,
                Activo = p.Activo
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, ProductoEditViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
        {
            await CargarProveedoresAsync(cancellationToken);
            return View(model);
        }

        await _productos.ActualizarAsync(
            new Producto
            {
                Id = id,
                IdProveedor = model.IdProveedor,
                Nombre = model.Nombre.Trim(),
                CodigoBarras = model.CodigoBarras,
                PrecioVenta = model.PrecioVenta,
                PrecioCosto = model.PrecioCosto,
                Existencias = model.Existencias,
                Activo = model.Activo
            },
            cancellationToken);

        TempData["Message"] = "Producto actualizado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarProveedoresAsync(CancellationToken cancellationToken)
    {
        var prov = await _proveedores.ListarActivosAsync(cancellationToken);
        ViewBag.Proveedores = new SelectList(prov, "Id", "Nombre");
    }
}
