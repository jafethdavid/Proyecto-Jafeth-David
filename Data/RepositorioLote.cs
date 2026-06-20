using System;
using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioLote : ILoteRepositorio
    {
        private readonly string _conexion;

        public RepositorioLote(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<Lote> ObtenerTodos()
        {
            var lista = new List<Lote>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT l.IdLote, l.IdProducto, l.CodigoLote,
                                        l.FechaIngreso, l.FechaVencimiento,
                                        l.Cantidad, l.Unidades, l.Estatus,
                                        p.NombreProducto
                                 FROM Lotes l
                                 INNER JOIN Productos p 
                                        ON l.IdProducto = p.IdProducto";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearLote(reader));
                    }
                }
            }

            return lista;
        }

        public List<Lote> ObtenerPorProducto(int idProducto)
        {
            var lista = new List<Lote>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT l.IdLote, l.IdProducto, l.CodigoLote,
                                        l.FechaIngreso, l.FechaVencimiento,
                                        l.Cantidad, l.Unidades, l.Estatus,
                                        p.NombreProducto
                                 FROM Lotes l
                                 INNER JOIN Productos p 
                                        ON l.IdProducto = p.IdProducto
                                 WHERE l.IdProducto = @IdProducto";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearLote(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public Lote? ObtenerPorId(int id)
        {
            Lote? lote = null;

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT l.IdLote, l.IdProducto, l.CodigoLote,
                                        l.FechaIngreso, l.FechaVencimiento,
                                        l.Cantidad, l.Unidades, l.Estatus,
                                        p.NombreProducto
                                 FROM Lotes l
                                 INNER JOIN Productos p 
                                        ON l.IdProducto = p.IdProducto
                                 WHERE l.IdLote = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            lote = MapearLote(reader);
                    }
                }
            }

            return lote;
        }

        public void Insertar(Lote lote)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO Lotes
                                    (IdProducto, CodigoLote, FechaIngreso,
                                     FechaVencimiento, Cantidad, Unidades, Estatus)
                                 VALUES
                                    (@IdProducto, @CodigoLote, @FechaIngreso,
                                     @FechaVencimiento, @Cantidad, @Unidades, @Estatus);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdProducto", lote.IdProducto);
                    cmd.Parameters.AddWithValue("@CodigoLote", lote.CodigoLote);
                    cmd.Parameters.AddWithValue("@FechaIngreso", lote.FechaIngreso);
                    cmd.Parameters.AddWithValue("@FechaVencimiento", (object?)lote.FechaVencimiento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", lote.Cantidad);
                    cmd.Parameters.AddWithValue("@Unidades", lote.Unidades);
                    cmd.Parameters.AddWithValue("@Estatus", lote.Estatus);

                    var resultado = cmd.ExecuteScalar();
                    if (resultado != null)
                        lote.IdLote = Convert.ToInt32(resultado);
                }
            }
        }

        public void Actualizar(Lote lote)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Lotes
                                 SET FechaVencimiento = @FechaVencimiento,
                                     Cantidad         = @Cantidad,
                                     Unidades         = @Unidades,
                                     Estatus          = @Estatus
                                 WHERE IdLote = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", lote.IdLote);
                    cmd.Parameters.AddWithValue("@FechaVencimiento", (object?)lote.FechaVencimiento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cantidad", lote.Cantidad);
                    cmd.Parameters.AddWithValue("@Unidades", lote.Unidades);
                    cmd.Parameters.AddWithValue("@Estatus", lote.Estatus);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Lotes 
                                 SET Estatus = 0 
                                 WHERE IdLote = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Lote MapearLote(SqlDataReader reader)
        {
            var l = new Lote();
            l.IdLote = reader.GetInt32(0);
            l.IdProducto = reader.GetInt32(1);
            l.CodigoLote = reader.GetString(2);
            l.FechaIngreso = reader.GetDateTime(3);
            l.FechaVencimiento = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
            l.Cantidad = reader.GetInt32(5);
            l.Unidades = reader.GetString(6);
            l.Estatus = reader.GetBoolean(7);
            l.NombreProducto = reader.GetString(8);
            return l;
        }
    }
}