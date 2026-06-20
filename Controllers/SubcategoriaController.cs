using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class SubcategoriaController : Controller
    {
        private readonly ISubcategoriaRepositorio _repoSubcategoria;
        private readonly ICategoriaRepositorio _repoCategoria;
        private readonly IProductoRepositorio _repoProducto;

        public SubcategoriaController(
            ISubcategoriaRepositorio repoSubcategoria,
            ICategoriaRepositorio repoCategoria,
            IProductoRepositorio repoProducto)
        {
            _repoSubcategoria = repoSubcategoria;
            _repoCategoria = repoCategoria;
            _repoProducto = repoProducto;
        }

        public IActionResult Index()
        {
            var subcategorias = _repoSubcategoria.ObtenerTodas();
            ViewBag.Productos = _repoProducto.ObtenerTodos();
            return View(subcategorias);
        }

        public IActionResult Create(int? idCategoria)
        {
            var categoriasActivas = _repoCategoria.ObtenerTodas()
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
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría.");

            if (subcategoria.IdCategoria.HasValue &&
                _repoSubcategoria.ObtenerTodas().Any(s =>
                s.Nombre.ToLower() == subcategoria.Nombre.ToLower() &&
                s.IdCategoria == subcategoria.IdCategoria))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una subcategoría con ese nombre en esta categoría.");
            }

            if (ModelState.IsValid)
            {
                _repoSubcategoria.Insertar(subcategoria);
                TempData["Success"] = "Subcategoría creada correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();
            return View(subcategoria);
        }

        public IActionResult Edit(int id)
        {
            var sub = _repoSubcategoria.ObtenerPorId(id);
            if (sub == null) return RedirectToAction("Index");

            ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();
            return View(sub);
        }

        [HttpPost]
        public IActionResult Edit(Subcategoria subcategoria)
        {
            if (subcategoria.IdCategoria == null || subcategoria.IdCategoria == 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría.");

            if (subcategoria.IdCategoria.HasValue &&
                _repoSubcategoria.ObtenerTodas().Any(s =>
                s.Nombre.ToLower() == subcategoria.Nombre.ToLower() &&
                s.IdCategoria == subcategoria.IdCategoria &&
                s.IdSubcategoria != subcategoria.IdSubcategoria))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una subcategoría con ese nombre en esta categoría.");
            }

            if (ModelState.IsValid)
            {
                _repoSubcategoria.Actualizar(subcategoria);
                TempData["Success"] = "Subcategoría actualizada correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();
            return View(subcategoria);
        }

        public IActionResult Delete(int id)
        {
            var sub = _repoSubcategoria.ObtenerPorId(id);

            if (sub == null)
            {
                TempData["Error"] = "Subcategoría no encontrada.";
                return RedirectToAction("Index");
            }

            bool tieneProductosActivos = _repoProducto.ObtenerTodos()
                .Any(p => p.IdSubcategoria == sub.IdSubcategoria && p.Estado);

            if (tieneProductosActivos)
            {
                TempData["Error"] = "No se puede desactivar porque tiene productos activos. Desactívalos primero.";
                return RedirectToAction("Index");
            }

            if (!sub.Estado)
            {
                TempData["Error"] = "Esta subcategoría ya está inactiva.";
                return RedirectToAction("Index");
            }

            _repoSubcategoria.Eliminar(id);
            TempData["Success"] = "Subcategoría desactivada correctamente.";
            return RedirectToAction("Index");
        }
    }
}