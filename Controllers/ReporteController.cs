using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IProductoRepositorio _repoProducto;
        private readonly ILoteRepositorio _repoLote;
        private readonly IMovimientoRepositorio _repoMovimiento;
        private readonly IFacturaRepositorio _repoFactura;

        public ReporteController(
            IProductoRepositorio repoProducto,
            ILoteRepositorio repoLote,
            IMovimientoRepositorio repoMovimiento,
            IFacturaRepositorio repoFactura)
        {
            _repoProducto = repoProducto;
            _repoLote = repoLote;
            _repoMovimiento = repoMovimiento;
            _repoFactura = repoFactura;
        }

        public IActionResult Productos()
        {
            var productos = _repoProducto.ObtenerTodos();

            foreach (var p in productos)
                p.Lotes = _repoLote.ObtenerPorProducto(p.IdProducto);

            ViewBag.ProductosBajoStock = productos
                .Where(p => p.Estado && p.BajoStock())
                .ToList();

            return View(productos);
        }

        public IActionResult Lotes()
        {
            var lotes = _repoLote.ObtenerTodos();

            ViewBag.LotesProximosVencer = lotes
                .Where(l => l.Estatus
                         && l.FechaVencimiento.HasValue
                         && l.DiasParaVencer() >= 0
                         && l.DiasParaVencer() <= 30)
                .OrderBy(l => l.FechaVencimiento)
                .ToList();

            return View(lotes);
        }

        public IActionResult Movimientos()
        {
            var movimientos = _repoMovimiento.ObtenerTodos();

            ViewBag.TipoEntrada = EnumTipoMovimiento.Entrada;
            ViewBag.TipoSalida = EnumTipoMovimiento.Salida;

            return View(movimientos);
        }

        public IActionResult Ventas()
        {
            var facturas = _repoFactura.ObtenerTodas();
            return View(facturas);
        }

        public IActionResult Detalle(int id)
        {
            var factura = _repoFactura.ObtenerPorId(id);

            if (factura == null)
                return RedirectToAction("Ventas");

            return View(factura);
        }
    }
}