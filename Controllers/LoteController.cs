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
        private readonly ServicioInventario servicio = new ServicioInventario();

        public IActionResult Index(int? idProducto)
        {
            var lotes = idProducto.HasValue
                ? BaseDatosMemoria.Lotes.Where(l => l.IdProducto == idProducto).ToList()
                : BaseDatosMemoria.Lotes.ToList();

            foreach (var lote in lotes)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == lote.IdProducto);
                lote.NombreProducto = prod?.NombreProducto;
            }

            ViewBag.IdProductoFiltro = idProducto;
            ViewBag.Productos = BaseDatosMemoria.Productos
                .Where(p => p.Estado).ToList();

            return View(lotes);
        }

        public IActionResult Create(int idProducto)
        {
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == idProducto);

            if (producto == null)
                return RedirectToAction("Index", "Producto");

            if (!producto.Estado)
            {
                TempData["Error"] = "No se puede agregar un lote a un producto inactivo.";
                return RedirectToAction("Index", "Producto");
            }

            int totalLotes = BaseDatosMemoria.Lotes
                .Count(l => l.IdProducto == idProducto) + 1;

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
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == idProducto);

            if (producto == null)
                return RedirectToAction("Index", "Producto");

            if (lote.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad",
                    "La cantidad debe ser mayor a 0.");
            }

            if (lote.FechaVencimiento.HasValue &&
                lote.FechaVencimiento.Value.Date <= DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaVencimiento",
                    "La fecha de vencimiento debe ser una fecha futura.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IdProducto = idProducto;
                ViewBag.NombreProducto = producto.NombreProducto;
                ViewBag.CodigoLote = lote.CodigoLote;
                return View(lote);
            }

            int totalLotes = BaseDatosMemoria.Lotes
                .Count(l => l.IdProducto == idProducto) + 1;

            string prefijo = producto.NombreProducto.Length >= 3
                ? producto.NombreProducto.Substring(0, 3).ToUpper()
                : producto.NombreProducto.ToUpper();

            lote.CodigoLote = $"LOT-{prefijo}-{totalLotes:D3}";
            lote.FechaIngreso = DateTime.Now;

            servicio.RegistrarEntrada(idProducto, lote);
            TempData["Success"] = $"Lote registrado correctamente para {producto.NombreProducto}.";
            return RedirectToAction("Index", "Producto");
        }

        public IActionResult Edit(int id)
        {
            var lote = BaseDatosMemoria.Lotes
                .FirstOrDefault(l => l.IdLote == id);
            if (lote == null)
            {
                TempData["Error"] = "Lote no encontrado.";
                return RedirectToAction("Index");
            }

            var prod = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == lote.IdProducto);
            ViewBag.NombreProducto = prod?.NombreProducto;
            return View(lote);
        }

        [HttpPost]
        public IActionResult Edit(Lote lote)
        {
            if (lote.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad",
                    "La cantidad debe ser mayor a 0.");
            }

            if (lote.FechaVencimiento.HasValue &&
                lote.FechaVencimiento.Value.Date <= DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaVencimiento",
                    "La fecha de vencimiento debe ser una fecha futura.");
            }

            if (!ModelState.IsValid)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == lote.IdProducto);
                ViewBag.NombreProducto = prod?.NombreProducto;
                return View(lote);
            }

            var existente = BaseDatosMemoria.Lotes
                .FirstOrDefault(l => l.IdLote == lote.IdLote);

            if (existente != null)
            {
                existente.FechaVencimiento = lote.FechaVencimiento;
                existente.Cantidad = lote.Cantidad;
                existente.Unidades = lote.Unidades;
                existente.Estatus = lote.Estatus;
            }

            TempData["Success"] = "Lote actualizado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var lote = BaseDatosMemoria.Lotes
                .FirstOrDefault(l => l.IdLote == id);

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
            {
                TempData["Warning"] = $"Se desactivó el lote {lote.CodigoLote} con {lote.Cantidad} unidades disponibles. Ese stock ya no estará disponible.";
            }
            else
            {
                TempData["Success"] = "Lote desactivado correctamente.";
            }

            lote.Estatus = false;

            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == lote.IdProducto);
            if (producto != null)
            {
                var loteEnProducto = producto.Lotes
                    .FirstOrDefault(l => l.IdLote == lote.IdLote);
                if (loteEnProducto != null)
                    loteEnProducto.Estatus = false;
            }

            return RedirectToAction("Index");
        }
    }
}