using System;
using System.Collections.Generic;
using IS_161_Proyecto_Grupo2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IS_161_Proyecto_Grupo2.Data
{
    public class RepositorioFactura : IFacturaRepositorio
    {
        private readonly string _conexion;

        public RepositorioFactura(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("ConexionDB")!;
        }

        public List<Factura> ObtenerTodas()
        {
            var lista = new List<Factura>();

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT IdFactura, Fecha, Cliente,
                                        Subtotal, IVA, Total,
                                        EstatusFactura
                                 FROM Facturas
                                 ORDER BY Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearFactura(reader));
                    }
                }
            }

            return lista;
        }

        public Factura? ObtenerPorId(int id)
        {
            Factura? factura = null;

            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"SELECT IdFactura, Fecha, Cliente,
                                        Subtotal, IVA, Total,
                                        EstatusFactura
                                 FROM Facturas
                                 WHERE IdFactura = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            factura = MapearFactura(reader);
                    }
                }

                if (factura != null)
                {
                    string queryDetalles = @"SELECT d.IdDetalle, d.IdFactura,
                                                    d.IdProducto, d.Cantidad,
                                                    d.PrecioUnitario, d.SubtotalLinea,
                                                    p.NombreProducto
                                             FROM DetalleFacturas d
                                             INNER JOIN Productos p 
                                                    ON d.IdProducto = p.IdProducto
                                             WHERE d.IdFactura = @IdFactura";

                    using (SqlCommand cmdDet = new SqlCommand(queryDetalles, conn))
                    {
                        cmdDet.Parameters.AddWithValue("@IdFactura", id);

                        using (SqlDataReader readerDet = cmdDet.ExecuteReader())
                        {
                            while (readerDet.Read())
                            {
                                var detalle = new DetalleFactura();
                                detalle.IdDetalle = readerDet.GetInt32(0);
                                detalle.IdFactura = readerDet.GetInt32(1);
                                detalle.IdProducto = readerDet.GetInt32(2);
                                detalle.Cantidad = readerDet.GetInt32(3);
                                detalle.PrecioUnitario = readerDet.GetDecimal(4);
                                detalle.SubtotalLinea = readerDet.GetDecimal(5);
                                detalle.NombreProducto = readerDet.GetString(6);
                                factura.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }

            return factura;
        }

        public int Insertar(Factura factura)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"INSERT INTO Facturas
                                    (Fecha, Cliente, Subtotal,
                                     IVA, Total, EstatusFactura)
                                 VALUES
                                    (@Fecha, @Cliente, @Subtotal,
                                     @IVA, @Total, @Estatus);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                    cmd.Parameters.AddWithValue("@Cliente", (object?)factura.Cliente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
                    cmd.Parameters.AddWithValue("@IVA", factura.IVA);
                    cmd.Parameters.AddWithValue("@Total", factura.Total);
                    cmd.Parameters.AddWithValue("@Estatus", (int)factura.EstatusFactura);

                    var resultado = cmd.ExecuteScalar();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public void Actualizar(Factura factura)
        {
            using (SqlConnection conn = new SqlConnection(_conexion))
            {
                conn.Open();
                string query = @"UPDATE Facturas
                                 SET EstatusFactura = @Estatus,
                                     Subtotal       = @Subtotal,
                                     IVA            = @IVA,
                                     Total          = @Total
                                 WHERE IdFactura = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", factura.IdFactura);
                    cmd.Parameters.AddWithValue("@Estatus", (int)factura.EstatusFactura);
                    cmd.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
                    cmd.Parameters.AddWithValue("@IVA", factura.IVA);
                    cmd.Parameters.AddWithValue("@Total", factura.Total);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Factura MapearFactura(SqlDataReader reader)
        {
            var f = new Factura();
            f.IdFactura = reader.GetInt32(0);
            f.Fecha = reader.GetDateTime(1);
            f.Cliente = reader.IsDBNull(2) ? null : reader.GetString(2);
            f.Subtotal = reader.GetDecimal(3);
            f.IVA = reader.GetDecimal(4);
            f.Total = reader.GetDecimal(5);
            f.EstatusFactura = (EnumEstatusFactura)reader.GetInt32(6);
            return f;
        }
    }
}