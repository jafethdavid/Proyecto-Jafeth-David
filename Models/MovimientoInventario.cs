using System;
using System.ComponentModel.DataAnnotations;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class MovimientoInventario
    {
        private int idMovimiento;         
        private int idProducto;           
        private int idLote;               
        private DateTime fechaMovimiento;
        private EnumTipoMovimiento tipoMovimiento;
        private int cantidadDisponible;   

        public MovimientoInventario() { }

        
        public MovimientoInventario(
            int idProducto,
            int idLote,
            EnumTipoMovimiento tipoMovimiento,
            int cantidadDisponible)
        {
            this.idProducto = idProducto;
            this.idLote = idLote;
            this.fechaMovimiento = DateTime.Now; 
            this.tipoMovimiento = tipoMovimiento;
            this.cantidadDisponible = cantidadDisponible;
        }

        [Key]
        public int IdMovimiento
        {
            get { return idMovimiento; }
            set { idMovimiento = value; }
        }

        [Required]
        [Display(Name = "Producto")]
        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        [Required]
        [Display(Name = "Lote")]
        public int IdLote
        {
            get { return idLote; }
            set { idLote = value; }
        }

        [Display(Name = "Fecha del Movimiento")]
        public DateTime FechaMovimiento
        {
            get { return fechaMovimiento; }
            set { fechaMovimiento = value; }
        }

        [Display(Name = "Tipo de Movimiento")]
        public EnumTipoMovimiento TipoMovimiento
        {
            get { return tipoMovimiento; }
            set { tipoMovimiento = value; }
        }

        [Range(0, int.MaxValue)]
        [Display(Name = "Cantidad")]
        public int CantidadDisponible
        {
            get { return cantidadDisponible; }
            set { cantidadDisponible = value; }
        }

        public string? NombreProducto { get; set; }
        public string? CodigoLote { get; set; }
    }
}