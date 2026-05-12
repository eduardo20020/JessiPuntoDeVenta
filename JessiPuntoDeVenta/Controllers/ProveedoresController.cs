using JessiPuntoDeVenta.Data.Models;
using JessiPuntoDeVenta.Data.Repositories;
using JessiPuntoDeVenta.Models;
using Microsoft.AspNetCore.Mvc;

namespace JessiPuntoDeVenta.Controllers;

public class ProveedoresController : Controller
{
    private readonly IProveedorRepository _proveedores;

    public ProveedoresController(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var list = await _proveedores.ListarTodosAsync(cancellationToken);
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ProveedorEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProveedorEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var id = await _proveedores.CrearAsync(
            new Proveedor
            {
                Nombre = model.Nombre.Trim(),
                Telefono = string.IsNullOrWhiteSpace(model.Telefono) ? null : model.Telefono.Trim(),
                Notas = string.IsNullOrWhiteSpace(model.Notas) ? null : model.Notas.Trim(),
                Activo = model.Activo
            },
            cancellationToken);

        TempData["Message"] = $"Proveedor creado (id {id}).";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var p = await _proveedores.ObtenerPorIdAsync(id, cancellationToken);
        if (p is null)
            return NotFound();

        return View(
            new ProveedorEditViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Telefono = p.Telefono,
                Notas = p.Notas,
                Activo = p.Activo
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, ProveedorEditViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        await _proveedores.ActualizarAsync(
            new Proveedor
            {
                Id = id,
                Nombre = model.Nombre.Trim(),
                Telefono = string.IsNullOrWhiteSpace(model.Telefono) ? null : model.Telefono.Trim(),
                Notas = string.IsNullOrWhiteSpace(model.Notas) ? null : model.Notas.Trim(),
                Activo = model.Activo
            },
            cancellationToken);

        TempData["Message"] = "Proveedor actualizado.";
        return RedirectToAction(nameof(Index));
    }
}
