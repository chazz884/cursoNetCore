using System;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        public string Titulo { get; set; }
        public List<int> AutoresId { get; set; }
    }
}

