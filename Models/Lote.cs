using System;
using System.ComponentModel.DataAnnotations;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class Lote
    {
        private int idLote;             
        private int idProducto;           
        private string codigoLote;
        private DateTime fechaIngreso;
        private DateTime? fechaVencimiento; 
        private int cantidad;               
        private string unidades;            
        private bool estatus;               

        public Lote()
        {
            fechaIngreso = DateTime.Now;
            unidades = "unidad";        
            estatus = true;            
        }

        public Lote(string codigoLote, DateTime fechaIngreso, int cantidad)
        {
            this.codigoLote = codigoLote;
            this.fechaIngreso = fechaIngreso;
            this.cantidad = cantidad;
            this.unidades = "unidad";
            this.estatus = true;
        }

        [Key]
        public int IdLote
        {
            get { return idLote; }
            set { idLote = value; }
        }

        [Required(ErrorMessage = "El producto es obligatorio")]
        [Display(Name = "Producto")]
        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        [Required(ErrorMessage = "El código de lote es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Código de Lote")]
        public string CodigoLote
        {
            get { return codigoLote; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    codigoLote = value;
            }
        }

        [Display(Name = "Fecha de Ingreso")]
        public DateTime FechaIngreso
        {
            get { return fechaIngreso; }
            set { fechaIngreso = value; }
        }

        [Display(Name = "Fecha de Vencimiento")]
        public DateTime? FechaVencimiento
        {
            get { return fechaVencimiento; }
            set { fechaVencimiento = value; }
        }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, 999999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad
        {
            get { return cantidad; }
            set
            {
                if (value >= 0)
                    cantidad = value;
            }
        }

        [StringLength(20)]
        [Display(Name = "Unidades")]
        public string Unidades
        {
            get { return unidades; }
            set { unidades = value ?? "unidad"; }
        }

        [Display(Name = "Activo")]
        public bool Estatus
        {
            get { return estatus; }
            set { estatus = value; }
        }

        public string? NombreProducto { get; set; }

        public bool EstaVencido()
        {
            if (FechaVencimiento == null) return false;
            return FechaVencimiento.Value.Date < DateTime.Now.Date;
        }

        public int DiasParaVencer()
        {
            if (FechaVencimiento == null) return int.MaxValue;
            return (FechaVencimiento.Value.Date - DateTime.Now.Date).Days;
        }

        public int DuracionDias()
        {
            if (FechaVencimiento == null)
                return (DateTime.Now - FechaIngreso).Days;
            return (FechaVencimiento.Value - FechaIngreso).Days;
        }
    }
}