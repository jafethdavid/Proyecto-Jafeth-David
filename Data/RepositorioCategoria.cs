using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioCategoria : ICategoriaRepositorio
    {
        private readonly string _conexion;

        public RepositorioCategoria(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<Categoria> ObtenerTodas()
        {
            var lista = new List<Categoria>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT IdCategoria, NombreCategoria, 
                                        Descripcion, Estado 
                                 FROM Categorias";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cat = new Categoria();
                        cat.IdCategoria = reader.GetInt32(0);
                        cat.Nombre = reader.GetString(1);
                        cat.Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2);
                        cat.Estado = reader.GetBoolean(3);
                        lista.Add(cat);
                    }
                }
            }

            return lista;
        }

        public Categoria? ObtenerPorId(int id)
        {
            Categoria? categoria = null;

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT IdCategoria, NombreCategoria, 
                                        Descripcion, Estado 
                                 FROM Categorias 
                                 WHERE IdCategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new Categoria();
                            categoria.IdCategoria = reader.GetInt32(0);
                            categoria.Nombre = reader.GetString(1);
                            categoria.Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2);
                            categoria.Estado = reader.GetBoolean(3);
                        }
                    }
                }
            }

            return categoria;
        }

        public void Insertar(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO Categorias 
                                    (NombreCategoria, Descripcion, Estado)
                                 VALUES 
                                    (@Nombre, @Descripcion, @Estado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", categoria.Estado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Categorias 
                                 SET NombreCategoria = @Nombre,
                                     Descripcion     = @Descripcion,
                                     Estado          = @Estado
                                 WHERE IdCategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", categoria.IdCategoria);
                    cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", categoria.Estado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Categorias 
                                 SET Estado = 0 
                                 WHERE IdCategoria = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}