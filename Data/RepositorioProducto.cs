using System;
using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioProducto : IProductoRepositorio
    {
        private readonly string _conexion;

        public RepositorioProducto(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT p.IdProducto, p.NombreProducto,
                                        p.IdCategoria, p.IdSubcategoria,
                                        p.PrecioCosto, p.PrecioVenta,
                                        p.Impuesto, p.CodigoProducto,
                                        p.StockMinimo, p.Estado,
                                        p.FechaCreacion, p.ImagenUrl,
                                        c.NombreCategoria,
                                        s.NombreSubcategoria
                                 FROM Productos p
                                 INNER JOIN Categorias c 
                                        ON p.IdCategoria = c.IdCategoria
                                 INNER JOIN Subcategorias s 
                                        ON p.IdSubcategoria = s.IdSubcategoria";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearProducto(reader));
                    }
                }
            }

            return lista;
        }

        public List<Producto> ObtenerActivos()
        {
            var lista = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT p.IdProducto, p.NombreProducto,
                                        p.IdCategoria, p.IdSubcategoria,
                                        p.PrecioCosto, p.PrecioVenta,
                                        p.Impuesto, p.CodigoProducto,
                                        p.StockMinimo, p.Estado,
                                        p.FechaCreacion, p.ImagenUrl,
                                        c.NombreCategoria,
                                        s.NombreSubcategoria
                                 FROM Productos p
                                 INNER JOIN Categorias c 
                                        ON p.IdCategoria = c.IdCategoria
                                 INNER JOIN Subcategorias s 
                                        ON p.IdSubcategoria = s.IdSubcategoria
                                 WHERE p.Estado = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearProducto(reader));
                    }
                }
            }

            return lista;
        }

        public Producto? ObtenerPorId(int id)
        {
            Producto? producto = null;

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT p.IdProducto, p.NombreProducto,
                                        p.IdCategoria, p.IdSubcategoria,
                                        p.PrecioCosto, p.PrecioVenta,
                                        p.Impuesto, p.CodigoProducto,
                                        p.StockMinimo, p.Estado,
                                        p.FechaCreacion, p.ImagenUrl,
                                        c.NombreCategoria,
                                        s.NombreSubcategoria
                                 FROM Productos p
                                 INNER JOIN Categorias c 
                                        ON p.IdCategoria = c.IdCategoria
                                 INNER JOIN Subcategorias s 
                                        ON p.IdSubcategoria = s.IdSubcategoria
                                 WHERE p.IdProducto = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            producto = MapearProducto(reader);
                    }
                }
            }

            return producto;
        }

        public void Insertar(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO Productos
                                    (NombreProducto, IdCategoria, IdSubcategoria,
                                     PrecioCosto, PrecioVenta, Impuesto,
                                     CodigoProducto, StockMinimo, Estado,
                                     FechaCreacion, ImagenUrl)
                                 VALUES
                                    (@Nombre, @IdCategoria, @IdSubcategoria,
                                     @PrecioCosto, @PrecioVenta, @Impuesto,
                                     @Codigo, @StockMinimo, @Estado,
                                     @FechaCreacion, @ImagenUrl)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@IdSubcategoria", producto.IdSubcategoria);
                    cmd.Parameters.AddWithValue("@PrecioCosto", producto.PrecioCosto);
                    cmd.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    cmd.Parameters.AddWithValue("@Impuesto", producto.Impuesto);
                    cmd.Parameters.AddWithValue("@Codigo", producto.CodigoProducto);
                    cmd.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                    cmd.Parameters.AddWithValue("@Estado", producto.Estado);
                    cmd.Parameters.AddWithValue("@FechaCreacion", producto.FechaCreacion);
                    cmd.Parameters.AddWithValue("@ImagenUrl", (object?)producto.ImagenUrl ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Productos
                                 SET NombreProducto  = @Nombre,
                                     IdCategoria     = @IdCategoria,
                                     IdSubcategoria  = @IdSubcategoria,
                                     PrecioCosto     = @PrecioCosto,
                                     PrecioVenta     = @PrecioVenta,
                                     Impuesto        = @Impuesto,
                                     CodigoProducto  = @Codigo,
                                     StockMinimo     = @StockMinimo,
                                     Estado          = @Estado,
                                     ImagenUrl       = @ImagenUrl
                                 WHERE IdProducto = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@Nombre", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@IdSubcategoria", producto.IdSubcategoria);
                    cmd.Parameters.AddWithValue("@PrecioCosto", producto.PrecioCosto);
                    cmd.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    cmd.Parameters.AddWithValue("@Impuesto", producto.Impuesto);
                    cmd.Parameters.AddWithValue("@Codigo", producto.CodigoProducto);
                    cmd.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
                    cmd.Parameters.AddWithValue("@Estado", producto.Estado);
                    cmd.Parameters.AddWithValue("@ImagenUrl", (object?)producto.ImagenUrl ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Productos 
                                 SET Estado = 0 
                                 WHERE IdProducto = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Producto MapearProducto(SqlDataReader reader)
        {
            var p = new Producto();
            p.IdProducto = reader.GetInt32(0);
            p.NombreProducto = reader.GetString(1);
            p.IdCategoria = reader.GetInt32(2);
            p.IdSubcategoria = reader.GetInt32(3);
            p.PrecioCosto = reader.GetDecimal(4);
            p.PrecioVenta = reader.GetDecimal(5);
            p.Impuesto = reader.GetDecimal(6);
            p.CodigoProducto = reader.GetString(7);
            p.StockMinimo = reader.GetInt32(8);
            p.Estado = reader.GetBoolean(9);
            p.FechaCreacion = reader.GetDateTime(10);
            p.ImagenUrl = reader.IsDBNull(11) ? null : reader.GetString(11);
            p.NombreCategoria = reader.GetString(12);
            p.NombreSubcategoria = reader.GetString(13);
            return p;
        }
    }
}