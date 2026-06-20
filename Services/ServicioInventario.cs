using System;
using System.Collections.Generic;
using System.Linq;
using IS_161_Proyecto_Grupo2.Data;
using IS_161_Proyecto_Grupo2.Models;

namespace IS_161_Proyecto_Grupo2.Services
{
    public class ServicioInventario
    {
        public void RegistrarEntrada(int productoId, Lote lote)
        {
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == productoId);

            if (producto != null)
            {
                lote.IdLote = BaseDatosMemoria.Lotes.Any()
                    ? BaseDatosMemoria.Lotes.Max(l => l.IdLote) + 1 : 1;
                lote.IdProducto = productoId;
                lote.FechaIngreso = DateTime.Now;

                BaseDatosMemoria.Lotes.Add(lote);
                producto.Lotes.Add(lote);

                BaseDatosMemoria.Movimientos.Add(
                    new MovimientoInventario(
                        productoId,
                        lote.IdLote,              
                        EnumTipoMovimiento.Entrada,
                        lote.Cantidad             
                    )
                );
            }
        }

        public bool RegistrarSalida(int productoId, int cantidad)
        {
            var producto = BaseDatosMemoria.Productos
                .FirstOrDefault(p => p.IdProducto == productoId);

            if (producto == null) return false;

            var lotesDisponibles = producto.Lotes
                .Where(l => l.Estatus && !l.EstaVencido() && l.Cantidad > 0)
                .OrderBy(l => l.FechaIngreso)
                .ToList();

            int totalDisponible = lotesDisponibles.Sum(l => l.Cantidad);

            if (totalDisponible < cantidad) return false;

            int restante = cantidad;

            foreach (var lote in lotesDisponibles)
            {
                if (restante <= 0) break;

                int descontar = Math.Min(restante, lote.Cantidad);

                lote.Cantidad -= descontar;
                restante -= descontar;

                if (lote.Cantidad == 0)
                    lote.Estatus = false;


                var loteGlobal = BaseDatosMemoria.Lotes
                    .FirstOrDefault(l => l.IdLote == lote.IdLote);
                if (loteGlobal != null)
                {
                    loteGlobal.Cantidad = lote.Cantidad;
                    loteGlobal.Estatus = lote.Estatus;
                }

                BaseDatosMemoria.Movimientos.Add(
                    new MovimientoInventario(
                        productoId,
                        lote.IdLote,              
                        EnumTipoMovimiento.Salida,
                        descontar                 
                    )
                );
            }

            return true;
        }

        public List<Producto> ObtenerProductosBajoStock()
        {
            return BaseDatosMemoria.Productos
                .Where(p => p.Estado && p.BajoStock())
                .ToList();
        }

        public List<Lote> ObtenerLotesProximosAVencer(int diasAviso = 30)
        {
            return BaseDatosMemoria.Lotes
                .Where(l => l.Estatus
                         && l.FechaVencimiento.HasValue
                         && l.DiasParaVencer() >= 0
                         && l.DiasParaVencer() <= diasAviso)
                .OrderBy(l => l.FechaVencimiento)
                .ToList();
        }
    }
}