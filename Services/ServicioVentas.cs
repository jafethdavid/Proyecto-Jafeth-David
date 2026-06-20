using System.Linq;
using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;

namespace IS_161_Proyecto_Grupo2.Services
{
    public class ServicioVentas
    {
        private readonly ServicioInventario servicioInventario = new ServicioInventario();

        public bool RegistrarVenta(Factura factura)
        {
            foreach (var detalle in factura.Detalles)
            {
                var producto = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

                
                if (producto == null || !producto.Estado)
                    return false;

             
                if (producto.StockTotal() < detalle.Cantidad)
                    return false;
            }

            
            foreach (var detalle in factura.Detalles)
            {
                bool exito = servicioInventario.RegistrarSalida(
                    detalle.IdProducto, 
                    detalle.Cantidad
                );
                if (!exito) return false;

                
                detalle.CalcularSubtotal();
            }

            factura.CalcularTotales();

            factura.EstatusFactura = EnumEstatusFactura.Procesada;

            
            factura.IdFactura = BaseDatosMemoria.Facturas.Any()
                ? BaseDatosMemoria.Facturas.Max(f => f.IdFactura) + 1
                : 1;

            
            int idDetalle = BaseDatosMemoria.Facturas
                .SelectMany(f => f.Detalles).Count() + 1;

            foreach (var detalle in factura.Detalles)
            {
                detalle.IdFactura = factura.IdFactura;
                detalle.IdDetalle = idDetalle++;
            }

            BaseDatosMemoria.Facturas.Add(factura);
            return true;
        }

        
        public bool CancelarVenta(int idFactura)
        {
            var factura = BaseDatosMemoria.Facturas
                .FirstOrDefault(f => f.IdFactura == idFactura);

            if (factura == null ||
                factura.EstatusFactura == EnumEstatusFactura.Cancelada)
                return false;

            
            foreach (var detalle in factura.Detalles)
            {
                var producto = BaseDatosMemoria.Productos
                    .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

                if (producto != null)
                {
                    
                    var loteDev = new Lote
                    {
                        IdProducto = detalle.IdProducto,
                        CodigoLote = $"DEV-FAC{idFactura:D6}",
                        Cantidad = detalle.Cantidad,
                        Unidades = "unidad",
                        Estatus = true
                    };
                    servicioInventario.RegistrarEntrada(detalle.IdProducto, loteDev);
                }
            }

            factura.EstatusFactura = EnumEstatusFactura.Cancelada;
            return true;
        }
    }
}