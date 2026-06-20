using System.ComponentModel.DataAnnotations;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class DetalleFactura
    {
        private int idDetalle;         
        private int idFactura;         
        private int idProducto;        
        private int cantidad;
        private decimal precioUnitario;
        private decimal subtotalLinea; 

        public DetalleFactura() { }

        public DetalleFactura(int idFactura, int idProducto, int cantidad, decimal precioUnitario)
        {
            this.idFactura = idFactura;      
            this.idProducto = idProducto;      
            this.cantidad = cantidad;
            this.precioUnitario = precioUnitario;
            this.subtotalLinea = cantidad * precioUnitario; 
        }

        [Key]
        public int IdDetalle
        {
            get { return idDetalle; }
            set { idDetalle = value; }
        }

        [Required]
        [Display(Name = "Factura")]
        public int IdFactura
        {
            get { return idFactura; }
            set { idFactura = value; }
        }

        [Required(ErrorMessage = "El producto es obligatorio")]
        [Display(Name = "Producto")]
        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario
        {
            get { return precioUnitario; }
            set { precioUnitario = value; }
        }

        [Display(Name = "Subtotal")]
        public decimal SubtotalLinea
        {
            get { return subtotalLinea; }
            set { subtotalLinea = value; }
        }

      
        public void CalcularSubtotal()
        {
            subtotalLinea = cantidad * precioUnitario;
        }

        public string? NombreProducto { get; set; }
    }
}