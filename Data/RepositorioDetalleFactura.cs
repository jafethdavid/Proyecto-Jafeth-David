using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioDetalleFactura : IDetalleFacturaRepositorio
    {
        private readonly string _conexion;

        public RepositorioDetalleFactura(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<DetalleFactura> ObtenerPorFactura(int idFactura)
        {
            var lista = new List<DetalleFactura>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT d.IdDetalle, d.IdFactura,
                                        d.IdProducto, d.Cantidad,
                                        d.PrecioUnitario, d.SubtotalLinea,
                                        p.NombreProducto
                                 FROM DetalleFacturas d
                                 INNER JOIN Productos p 
                                        ON d.IdProducto = p.IdProducto
                                 WHERE d.IdFactura = @IdFactura";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdFactura", idFactura);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var detalle = new DetalleFactura();
                            detalle.IdDetalle = reader.GetInt32(0);
                            detalle.IdFactura = reader.GetInt32(1);
                            detalle.IdProducto = reader.GetInt32(2);
                            detalle.Cantidad = reader.GetInt32(3);
                            detalle.PrecioUnitario = reader.GetDecimal(4);
                            detalle.SubtotalLinea = reader.GetDecimal(5);
                            detalle.NombreProducto = reader.GetString(6);
                            lista.Add(detalle);
                        }
                    }
                }
            }

            return lista;
        }


        public void Insertar(DetalleFactura detalle)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO DetalleFacturas
                                    (IdFactura, IdProducto, Cantidad,
                                     PrecioUnitario, SubtotalLinea)
                                 VALUES
                                    (@IdFactura, @IdProducto, @Cantidad,
                                     @PrecioUnitario, @SubtotalLinea)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdFactura", detalle.IdFactura);
                    cmd.Parameters.AddWithValue("@IdProducto", detalle.IdProducto);
                    cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@SubtotalLinea", detalle.SubtotalLinea);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}