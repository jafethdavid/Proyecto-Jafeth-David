using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _repoCategoria;
        private readonly ISubcategoriaRepositorio _repoSubcategoria;
        private readonly IProductoRepositorio _repoProducto;

        public CategoriaController(
            ICategoriaRepositorio repoCategoria,
            ISubcategoriaRepositorio repoSubcategoria,
            IProductoRepositorio repoProducto)
        {
            _repoCategoria = repoCategoria;
            _repoSubcategoria = repoSubcategoria;
            _repoProducto = repoProducto;
        }

        public IActionResult Index()
        {
            var categorias = _repoCategoria.ObtenerTodas();

            foreach (var cat in categorias)
            {
                cat.Subcategorias = _repoSubcategoria
                    .ObtenerPorCategoria(cat.IdCategoria);
            }

            return View(categorias);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (_repoCategoria.ObtenerTodas().Any(c =>
                c.Nombre.ToLower() == categoria.Nombre.ToLower()))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una categoría con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                _repoCategoria.Insertar(categoria);
                TempData["Success"] = "Categoría creada correctamente.";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        public IActionResult Edit(int id)
        {
            var categoria = _repoCategoria.ObtenerPorId(id);
            if (categoria == null) return RedirectToAction("Index");

            categoria.Subcategorias = _repoSubcategoria
                .ObtenerPorCategoria(id);

            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(Categoria categoria)
        {
            if (_repoCategoria.ObtenerTodas().Any(c =>
                c.Nombre.ToLower() == categoria.Nombre.ToLower() &&
                c.IdCategoria != categoria.IdCategoria))
            {
                ModelState.AddModelError("Nombre",
                    "Ya existe una categoría con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                _repoCategoria.Actualizar(categoria);
                TempData["Success"] = "Categoría actualizada correctamente.";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        public IActionResult Delete(int id)
        {
            var categoria = _repoCategoria.ObtenerPorId(id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoría no encontrada.";
                return RedirectToAction("Index");
            }

            bool tieneSubcategoriasActivas = _repoSubcategoria
                .ObtenerTodas()
                .Any(s => s.IdCategoria == id && s.Estado);

            if (tieneSubcategoriasActivas)
            {
                TempData["Error"] = "No se puede desactivar la categoría porque tiene subcategorías activas. Desactívalas primero.";
                return RedirectToAction("Index");
            }

            bool tieneProductosActivos = _repoProducto
                .ObtenerTodos()
                .Any(p => p.IdCategoria == id && p.Estado);

            if (tieneProductosActivos)
            {
                TempData["Error"] = "No se puede desactivar la categoría porque tiene productos activos asociados. Desactívalos primero.";
                return RedirectToAction("Index");
            }

            _repoCategoria.Eliminar(id);
            TempData["Success"] = "Categoría desactivada correctamente.";
            return RedirectToAction("Index");
        }
    }
}