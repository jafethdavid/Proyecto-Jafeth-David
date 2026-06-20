using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class SubcategoriaController : Controller
    {
        public IActionResult Index()
        {
            var subcategorias = BaseDatosMemoria.Subcategorias.ToList();

            foreach (var sub in subcategorias)
            {
                var cat = BaseDatosMemoria.Categorias
                    .FirstOrDefault(c => c.IdCategoria == sub.IdCategoria);
                sub.NombreCategoria = cat?.Nombre ?? "Sin categoría";
            }

            return View(subcategorias);
        }

        public IActionResult Create(int? idCategoria)
        {
            var categoriasActivas = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();

            if (!categoriasActivas.Any())
            {
                TempData["Error"] = "No hay categorías activas. Crea una categoría primero.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = categoriasActivas;
            ViewBag.IdCategoriaSeleccionada = idCategoria;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Subcategoria subcategoria)
        {
            if (subcategoria.IdCategoria == null || subcategoria.IdCategoria == 0)
            {
                ModelState.AddModelError("IdCategoria",
                    "Debe seleccionar una categoría.");
            }

            if (subcategoria.IdCategoria.HasValue &&
                BaseDatosMemoria.Subcategorias.Any(s =>
                s.Nombre.ToLower() == subcategoria.Nombre.ToLower() &&
                s.IdCategoria == subcategoria.IdCategoria))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una subcategoría con ese nombre en esta categoría.");
            }

            if (ModelState.IsValid)
            {
                subcategoria.IdSubcategoria = BaseDatosMemoria.Subcategorias.Any()
                    ? BaseDatosMemoria.Subcategorias.Max(s => s.IdSubcategoria) + 1
                    : 1;

                var cat = BaseDatosMemoria.Categorias
                    .FirstOrDefault(c => c.IdCategoria == subcategoria.IdCategoria);
                if (cat != null)
                    cat.Subcategorias.Add(subcategoria);

                BaseDatosMemoria.Subcategorias.Add(subcategoria);

                TempData["Success"] = "Subcategoría creada correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();
            return View(subcategoria);
        }

        public IActionResult Edit(int id)
        {
            var sub = BaseDatosMemoria.Subcategorias
                .FirstOrDefault(s => s.IdSubcategoria == id);
            if (sub == null) return RedirectToAction("Index");

            ViewBag.Categorias = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();
            return View(sub);
        }

        [HttpPost]
        public IActionResult Edit(Subcategoria subcategoria)
        {
            if (subcategoria.IdCategoria == null || subcategoria.IdCategoria == 0)
            {
                ModelState.AddModelError("IdCategoria",
                    "Debe seleccionar una categoría.");
            }

            if (subcategoria.IdCategoria.HasValue &&
                BaseDatosMemoria.Subcategorias.Any(s =>
                s.Nombre.ToLower() == subcategoria.Nombre.ToLower() &&
                s.IdCategoria == subcategoria.IdCategoria &&
                s.IdSubcategoria != subcategoria.IdSubcategoria))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una subcategoría con ese nombre en esta categoría.");
            }

            if (ModelState.IsValid)
            {
                var existente = BaseDatosMemoria.Subcategorias
                    .FirstOrDefault(s => s.IdSubcategoria == subcategoria.IdSubcategoria);

                if (existente != null)
                {
                    existente.Nombre = subcategoria.Nombre;
                    existente.IdCategoria = subcategoria.IdCategoria;
                    existente.Descripcion = subcategoria.Descripcion;
                    existente.Estado = subcategoria.Estado;
                }

                TempData["Success"] = "Subcategoría actualizada correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();
            return View(subcategoria);
        }

        public IActionResult Delete(int id)
        {
            var sub = BaseDatosMemoria.Subcategorias
                .FirstOrDefault(s => s.IdSubcategoria == id);

            if (sub == null)
            {
                TempData["Error"] = "Subcategoría no encontrada.";
                return RedirectToAction("Index");
            }

            bool tieneProductosActivos = BaseDatosMemoria.Productos
                .Any(p => p.IdSubcategoria == sub.IdSubcategoria && p.Estado);

            if (tieneProductosActivos)
            {
                TempData["Error"] = "No se puede desactivar porque tiene productos activos asociados. Desactívalos primero.";
                return RedirectToAction("Index");
            }

            if (!sub.Estado)
            {
                TempData["Error"] = "Esta subcategoría ya está inactiva.";
                return RedirectToAction("Index");
            }

            sub.Estado = false;
            TempData["Success"] = "Subcategoría desactivada correctamente.";
            return RedirectToAction("Index");
        }
    }
}