using IS_161_Proyecto_Grupo2.Models;
using System.Collections.Generic;

namespace IS_161_Proyecto_Grupo2.Data
{
    public interface ICategoriaRepositorio
    {
        List<Categoria> ObtenerTodas();
        Categoria? ObtenerPorId(int id);
        void Insertar(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id);
    }

    public interface ISubcategoriaRepositorio
    {
        List<Subcategoria> ObtenerTodas();
        List<Subcategoria> ObtenerPorCategoria(int idCategoria);
        Subcategoria? ObtenerPorId(int id);
        void Insertar(Subcategoria subcategoria);
        void Actualizar(Subcategoria subcategoria);
        void Eliminar(int id);
    }

    public interface IProductoRepositorio
    {
        List<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        List<Producto> ObtenerActivos();
        void Insertar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
    }

    public interface ILoteRepositorio
    {
        List<Lote> ObtenerTodos();
        List<Lote> ObtenerPorProducto(int idProducto);
        Lote? ObtenerPorId(int id);
        void Insertar(Lote lote);
        void Actualizar(Lote lote);
        void Eliminar(int id);
    }

    public interface IMovimientoRepositorio
    {
        List<MovimientoInventario> ObtenerTodos();
        void Insertar(MovimientoInventario movimiento);
    }

    public interface IFacturaRepositorio
    {
        List<Factura> ObtenerTodas();
        Factura? ObtenerPorId(int id);
        int Insertar(Factura factura);
        void Actualizar(Factura factura);
    }

    public interface IDetalleFacturaRepositorio
    {
        List<DetalleFactura> ObtenerPorFactura(int idFactura);
        void Insertar(DetalleFactura detalle);
    }
}