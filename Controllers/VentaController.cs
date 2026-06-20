using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class VentaController : Controller
    {
        private readonly IFacturaRepositorio _repoFactura;
        private readonly IDetalleFacturaRepositorio _repoDetalle;
        private readonly IProductoRepositorio _repoProducto;
        private readonly ILoteRepositorio _repoLote;
        private readonly IMovimientoRepositorio _repoMovimiento;
        private readonly ICategoriaRepositorio _repoCategoria;
        private readonly ISubcategoriaRepositorio _repoSubcategoria;

        public VentaController(
            IFacturaRepositorio repoFactura,
            IDetalleFacturaRepositorio repoDetalle,
            IProductoRepositorio repoProducto,
            ILoteRepositorio repoLote,
            IMovimientoRepositorio repoMovimiento,
            ICategoriaRepositorio repoCategoria,
            ISubcategoriaRepositorio repoSubcategoria)
        {
            _repoFactura = repoFactura;
            _repoDetalle = repoDetalle;
            _repoProducto = repoProducto;
            _repoLote = repoLote;
            _repoMovimiento = repoMovimiento;
            _repoCategoria = repoCategoria;
            _repoSubcategoria = repoSubcategoria;
        }

        public IActionResult Index()
        {
            var facturas = _repoFactura.ObtenerTodas();
            return View(facturas);
        }

        public IActionResult Create()
        {
            var productos = _repoProducto.ObtenerActivos();

            foreach (var p in productos)
            {
                p.Lotes = _repoLote.ObtenerPorProducto(p.IdProducto)
                    .Where(l => l.Estatus && !l.EstaVencido())
                    .ToList();
            }

            if (!productos.Any())
            {
                TempData["Error"] = "No hay productos activos disponibles para vender.";
                return RedirectToAction("Index");
            }

            var productosConStock = productos.Where(p => p.StockTotal() > 0).ToList();
            var productosSinStock = productos.Where(p => p.StockTotal() == 0).ToList();

            if (!productosConStock.Any())
            {
                TempData["Error"] = "Todos los productos están sin stock. Registra lotes primero.";
                return RedirectToAction("Index");
            }

            if (productosSinStock.Any())
                TempData["Warning"] = $"{productosSinStock.Count} producto(s) no tienen stock y no aparecerán en la lista.";

            ViewBag.Productos = productosConStock;
            ViewBag.Categorias = _repoCategoria.ObtenerTodas()
                .Where(c => c.Estado).ToList();
            ViewBag.Subcategorias = _repoSubcategoria.ObtenerTodas()
                .Where(s => s.Estado).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Factura factura)
        {
            if (factura.Detalles == null || !factura.Detalles.Any())
            {
                TempData["Error"] = "Debe agregar al menos un producto a la venta.";
                return RedirectToAction("Create");
            }

            foreach (var detalle in factura.Detalles)
            {
                var producto = _repoProducto.ObtenerPorId(detalle.IdProducto);

                if (producto == null || !producto.Estado)
                {
                    TempData["Error"] = "Uno de los productos no existe o está inactivo.";
                    return RedirectToAction("Create");
                }

                if (detalle.Cantidad <= 0)
                {
                    TempData["Error"] = $"La cantidad de {producto.NombreProducto} debe ser mayor a 0.";
                    return RedirectToAction("Create");
                }

                producto.Lotes = _repoLote.ObtenerPorProducto(producto.IdProducto)
                    .Where(l => l.Estatus && !l.EstaVencido())
                    .ToList();

                if (producto.StockTotal() < detalle.Cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente para '{producto.NombreProducto}'. Disponible: {producto.StockTotal()}, solicitado: {detalle.Cantidad}.";
                    return RedirectToAction("Create");
                }

                detalle.PrecioUnitario = producto.PrecioVenta ?? 0;
                detalle.CalcularSubtotal();
            }

            factura.CalcularTotales();

            if (factura.MontoPagado.HasValue &&
                factura.MontoPagado.Value < factura.Total)
            {
                TempData["Error"] = $"El monto pagado (L. {factura.MontoPagado.Value:N2}) es menor al total (L. {factura.Total:N2}).";
                return RedirectToAction("Create");
            }

            factura.Fecha = DateTime.Now;
            factura.EstatusFactura = EnumEstatusFactura.Procesada;
            int idFactura = _repoFactura.Insertar(factura);
            factura.IdFactura = idFactura;

            foreach (var detalle in factura.Detalles)
            {
                detalle.IdFactura = idFactura;
                _repoDetalle.Insertar(detalle);

                DescontarStockFIFO(detalle.IdProducto, detalle.Cantidad);
            }

            TempData["Exito"] = $"Venta registrada correctamente: {factura.NumeroFactura}";
            return RedirectToAction("Detalle", new { id = idFactura });
        }

        public IActionResult Detalle(int id)
        {
            var factura = _repoFactura.ObtenerPorId(id);

            if (factura == null)
            {
                TempData["Error"] = "Factura no encontrada.";
                return RedirectToAction("Index");
            }

            return View(factura);
        }

        public IActionResult Cancelar(int id)
        {
            var factura = _repoFactura.ObtenerPorId(id);

            if (factura == null)
            {
                TempData["Error"] = "Factura no encontrada.";
                return RedirectToAction("Index");
            }

            if (factura.EstatusFactura == EnumEstatusFactura.Cancelada)
            {
                TempData["Error"] = "Esta factura ya está cancelada.";
                return RedirectToAction("Index");
            }

            foreach (var detalle in factura.Detalles)
            {
                var loteDevolucion = new Lote
                {
                    IdProducto = detalle.IdProducto,
                    CodigoLote = $"DEV-FAC{id:D6}",
                    Cantidad = detalle.Cantidad,
                    Unidades = "unidad",
                    Estatus = true,
                    FechaIngreso = DateTime.Now
                };

                _repoLote.Insertar(loteDevolucion);

                _repoMovimiento.Insertar(new MovimientoInventario(
                    detalle.IdProducto,
                    loteDevolucion.IdLote,
                    EnumTipoMovimiento.Entrada,
                    detalle.Cantidad
                ));
            }

            factura.EstatusFactura = EnumEstatusFactura.Cancelada;
            _repoFactura.Actualizar(factura);

            TempData["Exito"] = $"Factura {factura.NumeroFactura} cancelada y stock devuelto.";
            return RedirectToAction("Index");
        }

        private void DescontarStockFIFO(int idProducto, int cantidad)
        {
            var lotes = _repoLote.ObtenerPorProducto(idProducto)
                .Where(l => l.Estatus && !l.EstaVencido() && l.Cantidad > 0)
                .OrderBy(l => l.FechaIngreso)
                .ToList();

            int restante = cantidad;

            foreach (var lote in lotes)
            {
                if (restante <= 0) break;

                int descontar = Math.Min(restante, lote.Cantidad);
                lote.Cantidad -= descontar;
                restante -= descontar;

                if (lote.Cantidad == 0)
                    lote.Estatus = false;

                _repoLote.Actualizar(lote);

                _repoMovimiento.Insertar(new MovimientoInventario(
                    idProducto,
                    lote.IdLote,
                    EnumTipoMovimiento.Salida,
                    descontar
                ));
            }
        }
    }
}