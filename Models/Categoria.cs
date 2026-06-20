using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IS_161_Proyecto_Grupo2.Models
{
    public class Categoria
    {
        private int idCategoria;
        private string nombre;
        private string? descripcion;      
        private bool estado;              
        private List<Subcategoria> subcategorias;

        public Categoria()
        {
            subcategorias = new List<Subcategoria>();
            estado = true;              
        }

        public Categoria(int idCategoria, string nombre)
        {
            this.idCategoria = idCategoria;
            this.nombre = nombre;
            this.estado = true;           
            subcategorias = new List<Subcategoria>();
        }

        [Key]
        public int IdCategoria
        {
            get { return idCategoria; }
            set { idCategoria = value; }
        }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2-100 caracteres")]
        [Display(Name = "Nombre de Categoría")]
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    nombre = value;
            }
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

        public List<Subcategoria> Subcategorias
        {
            get { return subcategorias; }
            set { subcategorias = value; }
        }

    }
}