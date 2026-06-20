using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioSubcategoria : ISubcategoriaRepositorio
    {
        private readonly string _conexion;

        public RepositorioSubcategoria(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<Subcategoria> ObtenerTodas()
        {
            var lista = new List<Subcategoria>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT s.IdSubcategoria, s.NombreSubcategoria,
                                        s.IdCategoria, s.Descripcion, s.Estado,
                                        c.NombreCategoria
                                 FROM Subcategorias s
                                 INNER JOIN Categorias c 
                                        ON s.IdCategoria = c.IdCategoria";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sub = new Subcategoria();
                        sub.IdSubcategoria = reader.GetInt32(0);
                        sub.Nombre = reader.GetString(1);
                        sub.IdCategoria = reader.GetInt32(2);
                        sub.Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3);
                        sub.Estado = reader.GetBoolean(4);
                        sub.NombreCategoria = reader.GetString(5);
                        lista.Add(sub);
                    }
                }
            }

            return lista;
        }

        public List<Subcategoria> ObtenerPorCategoria(int idCategoria)
        {
            var lista = new List<Subcategoria>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT IdSubcategoria, NombreSubcategoria,
                                        IdCategoria, Descripcion, Estado
                                 FROM Subcategorias
                                 WHERE IdCategoria = @IdCategoria
                                 AND Estado = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sub = new Subcategoria();
                            sub.IdSubcategoria = reader.GetInt32(0);
                            sub.Nombre = reader.GetString(1);
                            sub.IdCategoria = reader.GetInt32(2);
                            sub.Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3);
                            sub.Estado = reader.GetBoolean(4);
                            lista.Add(sub);
                        }
                    }
                }
            }

            return lista;
        }

        public Subcategoria? ObtenerPorId(int id)
        {
            Subcategoria? sub = null;

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT s.IdSubcategoria, s.NombreSubcategoria,
                                        s.IdCategoria, s.Descripcion, s.Estado,
                                        c.NombreCategoria
                                 FROM Subcategorias s
                                 INNER JOIN Categorias c 
                                        ON s.IdCategoria = c.IdCategoria
                                 WHERE s.IdSubcategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sub = new Subcategoria();
                            sub.IdSubcategoria = reader.GetInt32(0);
                            sub.Nombre = reader.GetString(1);
                            sub.IdCategoria = reader.GetInt32(2);
                            sub.Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3);
                            sub.Estado = reader.GetBoolean(4);
                            sub.NombreCategoria = reader.GetString(5);
                        }
                    }
                }
            }

            return sub;
        }

        public void Insertar(Subcategoria subcategoria)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO Subcategorias
                                    (NombreSubcategoria, IdCategoria, Descripcion, Estado)
                                 VALUES
                                    (@Nombre, @IdCategoria, @Descripcion, @Estado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", subcategoria.Nombre);
                    cmd.Parameters.AddWithValue("@IdCategoria", subcategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)subcategoria.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", subcategoria.Estado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Subcategoria subcategoria)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Subcategorias
                                 SET NombreSubcategoria = @Nombre,
                                     IdCategoria        = @IdCategoria,
                                     Descripcion        = @Descripcion,
                                     Estado             = @Estado
                                 WHERE IdSubcategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", subcategoria.IdSubcategoria);
                    cmd.Parameters.AddWithValue("@Nombre", subcategoria.Nombre);
                    cmd.Parameters.AddWithValue("@IdCategoria", subcategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)subcategoria.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", subcategoria.Estado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Subcategorias 
                                 SET Estado = 0 
                                 WHERE IdSubcategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}