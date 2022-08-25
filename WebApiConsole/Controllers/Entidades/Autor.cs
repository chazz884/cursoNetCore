using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Controllers.Entidades
{
    public class Autor: IValidatableObject
    {
        // Validaciones predefinidas StringLength, CreditCard, Range, Required
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 115, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
        // [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        // [Range(18, 120)]
        // [NotMapped] // Lo que hace es no mapear en la base de datos cuando se modifique el modelo o entidad, se puede hacer para pruebas
        // public int edad { get; set; }
        // [CreditCard]
        // [NotMapped]
        // public string TarjetaCredito { get; set; }
        // [Url]
        // [NotMapped]
        // public string URL { get; set; }
        // public int Menor { get; set; }
        // public int Mayor { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] {nameof(Nombre)});
                }
            }

            // if (Menor > Mayor)
            // {
            //     yield return new ValidationResult("Este valor no puede ser más grande que el campo Mayor", new string[] {nameof(Menor)});
            // }
        }
    }
}

