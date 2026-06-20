using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;
using IS_161_Proyecto_Grupo2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Controllers
{
    public class VentaController : Controller
    {
        private readonly ServicioVentas servicioVentas = new ServicioVentas();

        public IActionResult Index()
        {
            var facturas = BaseDatosMemoria.Facturas
                .OrderByDescending(f => f.Fecha)
                .ToList();
            return View(facturas);
        }

        public IActionResult Create()
        {
            var productos = BaseDatosMemoria.Productos
                .Where(p => p.Estado)
                .ToList();

            foreach (var p in productos)
                p.Lotes = BaseDatosMemoria.Lotes
                    .Where(l => l.IdProducto == p.IdProducto
                             && l.Estatus
                             && !l.EstaVencido())
                    .ToList();

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
                TempData["Warning"] = $"{productosSinStock.Count} producto(s) no tienen stock disponible y no aparecerán en la lista.";

            ViewBag.Productos = productosConStock;
            ViewBag.Categorias = BaseDatosMemoria.Categorias
                .Where(c => c.Estado).ToList();
            ViewBag.Subcategorias = BaseDatosMemoria.Subcategorias
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
                var producto = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

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

                producto.Lotes = BaseDatosMemoria.Lotes
                    .Where(l => l.IdProducto == producto.IdProducto
                             && l.Estatus && !l.EstaVencido())
                    .ToList();

                if (producto.StockTotal() < detalle.Cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente para '{producto.NombreProducto}'. Disponible: {producto.StockTotal()}, solicitado: {detalle.Cantidad}.";
                    return RedirectToAction("Create");
                }

                detalle.PrecioUnitario = producto.PrecioVenta ?? 0;
            }

            decimal totalVenta = factura.Detalles
                .Sum(d => d.Cantidad * (d.PrecioUnitario > 0
                    ? d.PrecioUnitario
                    : BaseDatosMemoria.Productos
                        .FirstOrDefault(p => p.IdProducto == d.IdProducto)
                        ?.PrecioVenta ?? 0));

            decimal totalConIva = totalVenta + (totalVenta * 0.15m);

            if (factura.MontoPagado.HasValue && factura.MontoPagado.Value < totalConIva)
            {
                TempData["Error"] = $"El monto pagado (L. {factura.MontoPagado.Value:N2}) es menor al total de la factura (L. {totalConIva:N2}).";
                return RedirectToAction("Create");
            }

            bool ventaExitosa = servicioVentas.RegistrarVenta(factura);

            if (!ventaExitosa)
            {
                TempData["Error"] = "Stock insuficiente para uno o más productos.";
                return RedirectToAction("Create");
            }

            TempData["Exito"] = $"Venta registrada correctamente: {factura.NumeroFactura}";
            return RedirectToAction("Detalle", new { id = factura.IdFactura });
        }

        public IActionResult Detalle(int id)
        {
            var factura = BaseDatosMemoria.Facturas
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
            {
                TempData["Error"] = "Factura no encontrada.";
                return RedirectToAction("Index");
            }

            foreach (var det in factura.Detalles)
            {
                var prod = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == det.IdProducto);
                det.NombreProducto = prod?.NombreProducto;
            }
            return View(factura);
        }

        public IActionResult Cancelar(int id)
        {
            var factura = BaseDatosMemoria.Facturas
                .FirstOrDefault(f => f.IdFactura == id);

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

            bool cancelado = servicioVentas.CancelarVenta(id);

            TempData[cancelado ? "Exito" : "Error"] = cancelado
                ? $"Factura {factura.NumeroFactura} cancelada y stock devuelto correctamente."
                : "No se pudo cancelar la factura.";

            return RedirectToAction("Index");
        }
    }
}