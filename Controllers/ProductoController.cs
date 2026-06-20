using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _repoProducto;
        private readonly ICategoriaRepositorio _repoCategoria;
        private readonly ISubcategoriaRepositorio _repoSubcategoria;
        private readonly ILoteRepositorio _repoLote;

        public ProductoController(
            IProductoRepositorio repoProducto,
            ICategoriaRepositorio repoCategoria,
            ISubcategoriaRepositorio repoSubcategoria,
            ILoteRepositorio repoLote)
        {
            _repoProducto = repoProducto;
            _repoCategoria = repoCategoria;
            _repoSubcategoria = repoSubcategoria;
            _repoLote = repoLote;
        }

        public IActionResult Index()
        {
            var productos = _repoProducto.ObtenerTodos();

            foreach (var p in productos)
            {
                p.Lotes = _repoLote.ObtenerPorProducto(p.IdProducto);
            }

            return View(productos);
        }

        public IActionResult Create()
        {
            var categoriasActivas = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();

            if (!categoriasActivas.Any())
            {
                TempData["Error"] = "No hay categorías activas. Crea una categoría primero.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = categoriasActivas;
            ViewBag.Subcategorias = new List<Subcategoria>();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            if (producto.IdSubcategoria == 0)
                ModelState.AddModelError("IdSubcategoria", "Debe seleccionar una subcategoría.");

            if (producto.IdCategoria == 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría.");

            if (!string.IsNullOrEmpty(producto.CodigoProducto) &&
                _repoProducto.ObtenerTodos().Any(p =>
                p.CodigoProducto.ToLower() == producto.CodigoProducto.ToLower()))
            {
                ModelState.AddModelError("CodigoProducto",
                    "Ya existe un producto con ese código.");
            }

            if (producto.PrecioCosto.HasValue && producto.PrecioVenta.HasValue)
            {
                if (producto.PrecioVenta.Value < producto.PrecioCosto.Value)
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta no puede ser menor que el precio de costo.");

                if (producto.PrecioVenta.Value == producto.PrecioCosto.Value)
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta es igual al costo — el margen de ganancia será 0%.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                    .Where(c => c.Estado).ToList();
                ViewBag.Subcategorias = _repoSubcategoria
                    .ObtenerPorCategoria(producto.IdCategoria);
                return View(producto);
            }

            producto.FechaCreacion = DateTime.Now;
            _repoProducto.Insertar(producto);
            TempData["Success"] = "Producto creado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var producto = _repoProducto.ObtenerPorId(id);
            if (producto == null)
                return RedirectToAction("Index");

            ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();
            ViewBag.Subcategorias = _repoSubcategoria
                .ObtenerPorCategoria(producto.IdCategoria);
            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(Producto producto)
        {
            if (producto.IdSubcategoria == 0)
                ModelState.AddModelError("IdSubcategoria", "Debe seleccionar una subcategoría.");

            if (producto.IdCategoria == 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría.");

            if (!string.IsNullOrEmpty(producto.CodigoProducto) &&
                _repoProducto.ObtenerTodos().Any(p =>
                p.CodigoProducto.ToLower() == producto.CodigoProducto.ToLower() &&
                p.IdProducto != producto.IdProducto))
            {
                ModelState.AddModelError("CodigoProducto",
                    "Ya existe un producto con ese código.");
            }

            if (producto.PrecioCosto.HasValue && producto.PrecioVenta.HasValue)
            {
                if (producto.PrecioVenta.Value < producto.PrecioCosto.Value)
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta no puede ser menor que el precio de costo.");

                if (producto.PrecioVenta.Value == producto.PrecioCosto.Value)
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta es igual al costo — el margen de ganancia será 0%.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                    .Where(c => c.Estado).ToList();
                ViewBag.Subcategorias = _repoSubcategoria
                    .ObtenerPorCategoria(producto.IdCategoria);
                return View(producto);
            }

            _repoProducto.Actualizar(producto);
            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var producto = _repoProducto.ObtenerPorId(id);

            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Index");
            }

            bool tieneLotesActivos = _repoLote.ObtenerPorProducto(id)
                .Any(l => l.Estatus && l.Cantidad > 0);

            if (tieneLotesActivos)
            {
                TempData["Error"] = "No se puede desactivar el producto porque tiene lotes activos con stock.";
                return RedirectToAction("Index");
            }

            if (!producto.Estado)
            {
                TempData["Error"] = "Este producto ya está inactivo.";
                return RedirectToAction("Index");
            }

            _repoProducto.Eliminar(id);
            TempData["Success"] = "Producto desactivado correctamente.";
            return RedirectToAction("Index");
        }

        public JsonResult ObtenerSubcategorias(int idCategoria)
        {
            var subs = _repoSubcategoria
                .ObtenerPorCategoria(idCategoria)
                .Select(s => new { s.IdSubcategoria, s.Nombre })
                .ToList();
            return Json(subs);
        }
    }
}