using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class CategoriaController : Controller
    {
        public IActionResult Index()
        {
            foreach (var cat in BaseDatosMemoria.Categorias)
            {
                cat.Subcategorias = BaseDatosMemoria.Subcategorias
                    .Where(s => s.IdCategoria == cat.IdCategoria)
                    .ToList();
            }
            return View(BaseDatosMemoria.Categorias);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (BaseDatosMemoria.Categorias.Any(c =>
                c.Nombre.ToLower() == categoria.Nombre.ToLower()))
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                categoria.IdCategoria = BaseDatosMemoria.Categorias.Any()
                    ? BaseDatosMemoria.Categorias.Max(c => c.IdCategoria) + 1
                    : 1;

                BaseDatosMemoria.Categorias.Add(categoria);
                TempData["Success"] = "Categoría creada correctamente.";
                return RedirectToAction("Index");
            }
            return View(categoria);
        }

        public IActionResult Edit(int id)
        {
            var categoria = BaseDatosMemoria.Categorias
                .FirstOrDefault(c => c.IdCategoria == id);
            if (categoria == null) return RedirectToAction("Index");

            categoria.Subcategorias = BaseDatosMemoria.Subcategorias
                .Where(s => s.IdCategoria == id)
                .ToList();
            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(Categoria categoria)
        {
            if (BaseDatosMemoria.Categorias.Any(c =>
                c.Nombre.ToLower() == categoria.Nombre.ToLower() &&
                c.IdCategoria != categoria.IdCategoria))
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                var catExistente = BaseDatosMemoria.Categorias
                    .FirstOrDefault(c => c.IdCategoria == categoria.IdCategoria);

                if (catExistente != null)
                {
                    catExistente.Nombre = categoria.Nombre;
                    catExistente.Descripcion = categoria.Descripcion;
                    catExistente.Estado = categoria.Estado;
                }
                TempData["Success"] = "Categoría actualizada correctamente.";
                return RedirectToAction("Index");
            }
            return View(categoria);
        }

        public IActionResult Delete(int id)
        {
            var categoria = BaseDatosMemoria.Categorias
                .FirstOrDefault(c => c.IdCategoria == id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoría no encontrada.";
                return RedirectToAction("Index");
            }

            bool tieneSubcategoriasActivas = BaseDatosMemoria.Subcategorias
                .Any(s => s.IdCategoria == id && s.Estado);

            if (tieneSubcategoriasActivas)
            {
                TempData["Error"] = "No se puede desactivar la categoría porque tiene subcategorías activas. Desactívalas primero.";
                return RedirectToAction("Index");
            }

            bool tieneProductosActivos = BaseDatosMemoria.Productos
                .Any(p => p.IdCategoria == id && p.Estado);

            if (tieneProductosActivos)
            {
                TempData["Error"] = "No se puede desactivar la categoría porque tiene productos activos asociados. Desactívalos primero.";
                return RedirectToAction("Index");
            }

            categoria.Estado = false;
            TempData["Success"] = "Categoría desactivada correctamente.";
            return RedirectToAction("Index");
        }
    }
}