using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using IS_161_Proyecto_Grupo2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ServicioInventario servicioInventario = new ServicioInventario();

        public IActionResult Productos()
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

            ViewBag.ProductosBajoStock = servicioInventario
                .ObtenerProductosBajoStock();

            return View(productos);
        }

        public IActionResult Lotes()
        {
            var lotes = BaseDatosMemoria.Lotes.ToList();

            foreach (var l in lotes)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == l.IdProducto);
                l.NombreProducto = prod?.NombreProducto;
            }

            ViewBag.LotesProximosVencer = servicioInventario
                .ObtenerLotesProximosAVencer(30);

            return View(lotes);
        }

        public IActionResult Movimientos()
        {
            var movimientos = BaseDatosMemoria.Movimientos
                .OrderByDescending(m => m.FechaMovimiento) 
                .ToList();

            foreach (var m in movimientos)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == m.IdProducto);
                m.NombreProducto = prod?.NombreProducto;

                var lote = BaseDatosMemoria.Lotes
                    .FirstOrDefault(l => l.IdLote == m.IdLote);
                m.CodigoLote = lote?.CodigoLote;
            }

            ViewBag.TipoEntrada = EnumTipoMovimiento.Entrada;
            ViewBag.TipoSalida = EnumTipoMovimiento.Salida;

            return View(movimientos);
        }

        public IActionResult Ventas()
        {
            var facturas = BaseDatosMemoria.Facturas
                .OrderByDescending(f => f.Fecha) 
                .ToList();

            return View(facturas);
        }

        public IActionResult Detalle(int id)
        {
            var factura = BaseDatosMemoria.Facturas
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
                return RedirectToAction("Ventas");

            foreach (var det in factura.Detalles)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == det.IdProducto);
                det.NombreProducto = prod?.NombreProducto;
            }

            return View(factura);
        }
    }
}