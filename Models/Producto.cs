using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class Producto : IValidatableObject
    {
        private int idProducto;
        private string nombreProducto;
        private int idCategoria;          
        private int idSubcategoria;       
        private decimal? precioCosto;
        private decimal? precioVenta;
        private decimal impuesto;         
        private string codigoProducto;   
        private int stockMinimo;          
        private bool estado;              
        private DateTime fechaCreacion;   
        private string? imagenUrl;       
        private List<Lote> lotes;

        public Producto()
        {
            lotes = new List<Lote>();
            estado = true;                
            impuesto = 15.00m;            
            fechaCreacion = DateTime.Now; 
        }

        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2-150 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string NombreProducto
        {
            get { return nombreProducto; }
            set { nombreProducto = value; }
        }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        public int IdCategoria
        {
            get { return idCategoria; }
            set { idCategoria = value; }
        }

        [Required(ErrorMessage = "La subcategoría es obligatoria")]
        [Display(Name = "Subcategoría")]
        public int IdSubcategoria
        {
            get { return idSubcategoria; }
            set { idSubcategoria = value; }
        }

        [Required(ErrorMessage = "El precio de costo es obligatorio")]
        [Range(0.01, 999999, ErrorMessage = "El precio de costo debe ser mayor a 0")]
        [Display(Name = "Precio de Costo")]
        public decimal? PrecioCosto
        {
            get { return precioCosto; }
            set { precioCosto = value; }
        }

        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(0.01, 999999, ErrorMessage = "El precio de venta debe ser mayor a 0")]
        [Display(Name = "Precio de Venta")]
        public decimal? PrecioVenta
        {
            get { return precioVenta; }
            set { precioVenta = value; }
        }

        [Required(ErrorMessage = "El impuesto es obligatorio")]
        [Range(0, 100, ErrorMessage = "El impuesto debe estar entre 0 y 100")]
        [Display(Name = "Impuesto (%)")]
        public decimal Impuesto
        {
            get { return impuesto; }
            set { impuesto = value; }
        }

        [Required(ErrorMessage = "El código de producto es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Código (SKU)")]
        public string CodigoProducto
        {
            get { return codigoProducto; }
            set { codigoProducto = value; }
        }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo
        {
            get { return stockMinimo; }
            set { stockMinimo = value; }
        }

        [Display(Name = "Estado")]
        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }

        [StringLength(255)]
        [Display(Name = "Imagen URL")]
        public string? ImagenUrl
        {
            get { return imagenUrl; }
            set { imagenUrl = value; }
        }

        public string? NombreCategoria { get; set; }
        public string? NombreSubcategoria { get; set; }

        public List<Lote> Lotes
        {
            get { return lotes; }
            set { lotes = value; }
        }

        public int StockTotal()
        {
            return lotes.Where(l => l.Estatus).Sum(l => l.Cantidad);
        }

        public bool BajoStock()
        {
            return StockTotal() <= stockMinimo;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PrecioCosto.HasValue && PrecioVenta.HasValue)
            {
                if (PrecioVenta.Value < PrecioCosto.Value)
                {
                    yield return new ValidationResult(
                        "El precio de venta no puede ser menor que el precio de costo.",
                        new[] { nameof(PrecioVenta) }
                    );
                }
            }
        }
    }
}