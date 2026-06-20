using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using IS_161_Proyecto_Grupo2.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class LoteController : Controller
    {
        private readonly ILoteRepositorio _repoLote;
        private readonly IProductoRepositorio _repoProducto;
        private readonly IMovimientoRepositorio _repoMovimiento;

        public LoteController(
            ILoteRepositorio repoLote,
            IProductoRepositorio repoProducto,
            IMovimientoRepositorio repoMovimiento)
        {
            _repoLote = repoLote;
            _repoProducto = repoProducto;
            _repoMovimiento = repoMovimiento;
        }

        public IActionResult Index(int? idProducto)
        {

            var lotes = idProducto.HasValue
                ? _repoLote.ObtenerPorProducto(idProducto.Value)
                : _repoLote.ObtenerTodos();

            ViewBag.IdProductoFiltro = idProducto;
            ViewBag.Productos = _repoProducto.ObtenerActivos();

            return View(lotes);
        }

        public IActionResult Create(int idProducto)
        {
            var producto = _repoProducto.ObtenerPorId(idProducto);

            if (producto == null)
                return RedirectToAction("Index", "Producto");

            if (!producto.Estado)
            {
                TempData["Error"] = "No se puede agregar un lote a un producto inactivo.";
                return RedirectToAction("Index", "Producto");
            }

            int totalLotes = _repoLote.ObtenerPorProducto(idProducto).Count + 1;

            string prefijo = producto.NombreProducto.Length >= 3
                ? producto.NombreProducto.Substring(0, 3).ToUpper()
                : producto.NombreProducto.ToUpper();

            string codigo = $"LOT-{prefijo}-{totalLotes:D3}";

            ViewBag.IdProducto = idProducto;
            ViewBag.NombreProducto = producto.NombreProducto;
            ViewBag.CodigoLote = codigo;
            return View(new Lote());
        }

        [HttpPost]
        public IActionResult Create(int idProducto, Lote lote)
        {
            var producto = _repoProducto.ObtenerPorId(idProducto);

            if (producto == null)
                return RedirectToAction("Index", "Producto");

            if (lote.Cantidad <= 0)
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");

            if (lote.FechaVencimiento.HasValue &&
                lote.FechaVencimiento.Value.Date <= DateTime.Now.Date)
                ModelState.AddModelError("FechaVencimiento",
                    "La fecha de vencimiento debe ser una fecha futura.");

            if (!ModelState.IsValid)
            {
                ViewBag.IdProducto = idProducto;
                ViewBag.NombreProducto = producto.NombreProducto;
                ViewBag.CodigoLote = lote.CodigoLote;
                return View(lote);
            }

            int totalLotes = _repoLote.ObtenerPorProducto(idProducto).Count + 1;

            string prefijo = producto.NombreProducto.Length >= 3
                ? producto.NombreProducto.Substring(0, 3).ToUpper()
                : producto.NombreProducto.ToUpper();

            lote.CodigoLote = $"LOT-{prefijo}-{totalLotes:D3}";
            lote.FechaIngreso = DateTime.Now;
            lote.IdProducto = idProducto;
            lote.Estatus = true;

            _repoLote.Insertar(lote);

            _repoMovimiento.Insertar(new MovimientoInventario(
                idProducto,
                lote.IdLote,
                EnumTipoMovimiento.Entrada,
                lote.Cantidad
            ));

            TempData["Success"] = $"Lote registrado correctamente para {producto.NombreProducto}.";
            return RedirectToAction("Index", "Producto");
        }

        public IActionResult Edit(int id)
        {
            var lote = _repoLote.ObtenerPorId(id);
            if (lote == null)
            {
                TempData["Error"] = "Lote no encontrado.";
                return RedirectToAction("Index");
            }

            ViewBag.NombreProducto = lote.NombreProducto;
            return View(lote);
        }

        [HttpPost]
        public IActionResult Edit(Lote lote)
        {
            if (lote.Cantidad <= 0)
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");

            if (lote.FechaVencimiento.HasValue &&
                lote.FechaVencimiento.Value.Date <= DateTime.Now.Date)
                ModelState.AddModelError("FechaVencimiento",
                    "La fecha de vencimiento debe ser una fecha futura.");

            if (!ModelState.IsValid)
            {
                var loteActual = _repoLote.ObtenerPorId(lote.IdLote);
                ViewBag.NombreProducto = loteActual?.NombreProducto;
                return View(lote);
            }

            _repoLote.Actualizar(lote);
            TempData["Success"] = "Lote actualizado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var lote = _repoLote.ObtenerPorId(id);

            if (lote == null)
            {
                TempData["Error"] = "Lote no encontrado.";
                return RedirectToAction("Index");
            }

            if (!lote.Estatus)
            {
                TempData["Error"] = "Este lote ya está inactivo.";
                return RedirectToAction("Index");
            }

            if (lote.Cantidad > 0)
                TempData["Warning"] = $"Se desactivó el lote {lote.CodigoLote} con {lote.Cantidad} unidades disponibles. Ese stock ya no estará disponible.";
            else
                TempData["Success"] = "Lote desactivado correctamente.";

            _repoLote.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}