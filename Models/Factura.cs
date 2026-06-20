using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class Factura
    {
        private int idFactura;
        private DateTime fecha;
        private string? cliente;              
        private decimal subtotal;             
        private decimal iva;                  
        private decimal total;
        private decimal? montoPagado;
        private EnumEstatusFactura estatusFactura; 
        private List<DetalleFactura> detalles;

        public Factura()
        {
            fecha = DateTime.Now;
            estatusFactura = EnumEstatusFactura.Creada; 
            detalles = new List<DetalleFactura>();
        }

        public Factura(int idFactura, DateTime fecha)
        {
            this.idFactura = idFactura;
            this.fecha = fecha;
            this.estatusFactura = EnumEstatusFactura.Creada; 
            detalles = new List<DetalleFactura>();
        }

        [Key]
        public int IdFactura
        {
            get { return idFactura; }
            set { idFactura = value; }
        }

        [Display(Name = "Fecha")]
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

        [StringLength(200)]
        [Display(Name = "Cliente")]
        public string? Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        [Display(Name = "Subtotal")]
        public decimal Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }

        [Display(Name = "IVA (15%)")]
        public decimal IVA
        {
            get { return iva; }
            set { iva = value; }
        }

        [Display(Name = "Total")]
        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }

        [Display(Name = "Estado")]
        public EnumEstatusFactura EstatusFactura
        {
            get { return estatusFactura; }
            set { estatusFactura = value; }
        }

        public List<DetalleFactura> Detalles
        {
            get { return detalles; }
            set { detalles = value ?? new List<DetalleFactura>(); }
        }

        [Display(Name = "Monto Pagado")]
        public decimal? MontoPagado
        {
            get { return montoPagado; }
            set { montoPagado = value; }
        }
        public void CalcularTotales()
        {
            subtotal = detalles.Sum(d => d.SubtotalLinea);
            iva = Math.Round(subtotal * 0.15m, 2);
            total = subtotal + iva;
        }

        public string NumeroFactura => $"FAC-{idFactura:D6}";
    }

    public enum EnumEstatusFactura
    {
        Creada = 1,
        EnProceso = 2,
        Procesada = 3,
        Cancelada = 4
    }
}