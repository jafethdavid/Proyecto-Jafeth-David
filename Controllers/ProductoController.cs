using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            var productos = BaseDatosMemoria.Productos.ToList();

            foreach (var p in productos)
            {
                p.Lotes = BaseDatosMemoria.Lotes
                    .Where(l => l.IdProducto == p.IdProducto).ToList();

                var cat = BaseDatosMemoria.Categorias
                    .FirstOrDefault(c => c.IdCategoria == p.IdCategoria);
                p.NombreCategoria = cat?.Nombre;

                var sub = BaseDatosMemoria.Subcategorias
                    .FirstOrDefault(s => s.IdSubcategoria == p.IdSubcategoria);
                p.NombreSubcategoria = sub?.Nombre;
            }
            return View(productos);
        }

        public IActionResult Create()
        {
            var categoriasActivas = BaseDatosMemoria.Categorias
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
            {
                ModelState.AddModelError("IdSubcategoria",
                    "Debe seleccionar una subcategoría.");
            }

            if (producto.IdCategoria == 0)
            {
                ModelState.AddModelError("IdCategoria",
                    "Debe seleccionar una categoría.");
            }

            if (!string.IsNullOrEmpty(producto.CodigoProducto) &&
                BaseDatosMemoria.Productos.Any(p =>
                p.CodigoProducto.ToLower() == producto.CodigoProducto.ToLower()))
            {
                ModelState.AddModelError("CodigoProducto",
                    "Ya existe un producto con ese código.");
            }

            if (producto.PrecioCosto.HasValue && producto.PrecioVenta.HasValue)
            {
                if (producto.PrecioVenta.Value < producto.PrecioCosto.Value)
                {
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta no puede ser menor que el precio de costo.");
                }

                if (producto.PrecioVenta.Value == producto.PrecioCosto.Value)
                {
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta es igual al costo — el margen de ganancia será 0%.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = BaseDatosMemoria.Categorias
                    .Where(c => c.Estado).ToList();
                ViewBag.Subcategorias = BaseDatosMemoria.Subcategorias
                    .Where(s => s.IdCategoria == producto.IdCategoria && s.Estado).ToList();
                return View(producto);
            }

            producto.IdProducto = BaseDatosMemoria.Productos.Any()
                ? BaseDatosMemoria.Productos.Max(p => p.IdProducto) + 1
                : 1;

            if (string.IsNullOrWhiteSpace(producto.CodigoProducto))
                producto.CodigoProducto = producto.IdProducto.ToString();

            producto.FechaCreacion = DateTime.Now;
            BaseDatosMemoria.Productos.Add(producto);
            TempData["Success"] = "Producto creado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == id);
            if (producto == null)
                return RedirectToAction("Index");

            ViewBag.Categorias = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();
            ViewBag.Subcategorias = BaseDatosMemoria.Subcategorias
                .Where(s => s.IdCategoria == producto.IdCategoria && s.Estado).ToList();
            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(Producto producto)
        {
            if (producto.IdSubcategoria == 0)
            {
                ModelState.AddModelError("IdSubcategoria",
                    "Debe seleccionar una subcategoría.");
            }

            if (producto.IdCategoria == 0)
            {
                ModelState.AddModelError("IdCategoria",
                    "Debe seleccionar una categoría.");
            }

            if (!string.IsNullOrEmpty(producto.CodigoProducto) &&
                BaseDatosMemoria.Productos.Any(p =>
                p.CodigoProducto.ToLower() == producto.CodigoProducto.ToLower() &&
                p.IdProducto != producto.IdProducto))
            {
                ModelState.AddModelError("CodigoProducto",
                    "Ya existe un producto con ese código.");
            }

            if (producto.PrecioCosto.HasValue && producto.PrecioVenta.HasValue)
            {
                if (producto.PrecioVenta.Value < producto.PrecioCosto.Value)
                {
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta no puede ser menor que el precio de costo.");
                }

                if (producto.PrecioVenta.Value == producto.PrecioCosto.Value)
                {
                    ModelState.AddModelError("PrecioVenta",
                        "El precio de venta es igual al costo — el margen de ganancia será 0%.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = BaseDatosMemoria.Categorias
                    .Where(c => c.Estado).ToList();
                ViewBag.Subcategorias = BaseDatosMemoria.Subcategorias
                    .Where(s => s.IdCategoria == producto.IdCategoria && s.Estado).ToList();
                return View(producto);
            }

            var prod = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == producto.IdProducto);

            if (prod != null)
            {
                prod.NombreProducto = producto.NombreProducto;
                prod.IdCategoria = producto.IdCategoria;
                prod.IdSubcategoria = producto.IdSubcategoria;
                prod.PrecioCosto = producto.PrecioCosto;
                prod.PrecioVenta = producto.PrecioVenta;
                prod.Impuesto = producto.Impuesto;
                prod.CodigoProducto = producto.CodigoProducto;
                prod.StockMinimo = producto.StockMinimo;
                prod.Estado = producto.Estado;
                prod.ImagenUrl = producto.ImagenUrl;
            }

            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == id);

            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction("Index");
            }

            bool tieneLotesActivos = BaseDatosMemoria.Lotes
                .Any(l => l.IdProducto == id && l.Estatus && l.Cantidad > 0);

            if (tieneLotesActivos)
            {
                TempData["Error"] = "No se puede desactivar el producto porque tiene lotes activos con stock. Desactívalos primero.";
                return RedirectToAction("Index");
            }

            if (!producto.Estado)
            {
                TempData["Error"] = "Este producto ya está inactivo.";
                return RedirectToAction("Index");
            }

            producto.Estado = false;
            TempData["Success"] = "Producto desactivado correctamente.";
            return RedirectToAction("Index");
        }

        public JsonResult ObtenerSubcategorias(int idCategoria)
        {
            var subs = BaseDatosMemoria.Subcategorias
                .Where(s => s.IdCategoria == idCategoria && s.Estado)
                .Select(s => new { s.IdSubcategoria, s.Nombre })
                .ToList();
            return Json(subs);
        }
    }
}