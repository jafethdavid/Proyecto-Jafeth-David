using System.ComponentModel.DataAnnotations;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class Subcategoria
    {
        private int idSubcategoria;
        private string nombre;
        private int? idCategoria;
        private string? descripcion; 
        private bool estado;        

        public Subcategoria()
        {
            estado = true;            
        }

        public Subcategoria(int idSubcategoria, string nombre, int idCategoria)
        {
            this.idSubcategoria = idSubcategoria;
            this.nombre = nombre;
            this.idCategoria = idCategoria;
            this.estado = true;       
        }

        [Key]
        public int IdSubcategoria
        {
            get { return idSubcategoria; }
            set { idSubcategoria = value; }
        }

        [Required(ErrorMessage = "El nombre de la subcategoría es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2-100 caracteres")]
        [Display(Name = "Nombre de Subcategoría")]
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    nombre = value;
            }
        }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int? IdCategoria
        {
            get { return idCategoria; }
            set { idCategoria = value; }
        }

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        [Display(Name = "Estado")]
        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public string? NombreCategoria { get; set; }
    }
}