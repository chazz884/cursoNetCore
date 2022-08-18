using System;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Controllers.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutorLibros { get; set; }

    }
}

