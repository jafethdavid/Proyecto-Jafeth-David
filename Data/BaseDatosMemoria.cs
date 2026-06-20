using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;

namespace IS_161_Proyecto_Grupo2.Data
{
    public static class BaseDatosMemoria
    {
        public static List<Categoria> Categorias = new List<Categoria>();
        public static List<Subcategoria> Subcategorias = new List<Subcategoria>(); // ── NUEVO
        public static List<Producto> Productos = new List<Producto>();
        public static List<Lote> Lotes = new List<Lote>();         // ── NUEVO
        public static List<MovimientoInventario> Movimientos = new List<MovimientoInventario>();
        public static List<Factura> Facturas = new List<Factura>();
    }
}