using System;
using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioMovimiento : IMovimientoRepositorio
    {
        private readonly string _conexion;

        public RepositorioMovimiento(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<MovimientoInventario> ObtenerTodos()
        {
            var lista = new List<MovimientoInventario>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT m.IdMovimiento, m.IdProducto, m.IdLote,
                                        m.FechaMovimiento, m.TipoMovimiento,
                                        m.CantidadDisponible,
                                        p.NombreProducto,
                                        l.CodigoLote
                                 FROM MovimientosInventario m
                                 INNER JOIN Productos p 
                                        ON m.IdProducto = p.IdProducto
                                 INNER JOIN Lotes l 
                                        ON m.IdLote = l.IdLote
                                 ORDER BY m.FechaMovimiento DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var mov = new MovimientoInventario();
                        mov.IdMovimiento = reader.GetInt32(0);
                        mov.IdProducto = reader.GetInt32(1);
                        mov.IdLote = reader.GetInt32(2);
                        mov.FechaMovimiento = reader.GetDateTime(3);
                        mov.TipoMovimiento = (EnumTipoMovimiento)reader.GetInt32(4);
                        mov.CantidadDisponible = reader.GetInt32(5);
                        mov.NombreProducto = reader.GetString(6);
                        mov.CodigoLote = reader.GetString(7);
                        lista.Add(mov);
                    }
                }
            }

            return lista;
        }

        public void Insertar(MovimientoInventario movimiento)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO MovimientosInventario
                                    (IdProducto, IdLote, FechaMovimiento,
                                     TipoMovimiento, CantidadDisponible)
                                 VALUES
                                    (@IdProducto, @IdLote, @FechaMovimiento,
                                     @TipoMovimiento, @CantidadDisponible)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdProducto", movimiento.IdProducto);
                    cmd.Parameters.AddWithValue("@IdLote", movimiento.IdLote);
                    cmd.Parameters.AddWithValue("@FechaMovimiento", movimiento.FechaMovimiento);
                    cmd.Parameters.AddWithValue("@TipoMovimiento", (int)movimiento.TipoMovimiento);
                    cmd.Parameters.AddWithValue("@CantidadDisponible", movimiento.CantidadDisponible);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}