using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Controllers.Entidades
{
    public class Autor
    {
        // Validaciones predefinidas StringLength, CreditCard, Range, Required
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 115, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
    }
}

