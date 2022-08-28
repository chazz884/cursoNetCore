
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebAPIConsole.Tests.PruebasUnitarias;

[TestClass]
public class PrimeraLetraMayusculaAttributeTests
{
    [TestMethod]
    public void PrimeraLetraMinusculaDevuelveError()
    {
        // preparación
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); 
        var valor = "felipe";
        var valContext = new ValidationContext( new {Nombre = valor});

        // Ejecución
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificación
        Assert.AreEqual("La primera letra debe ser nayúscula", resultado.ErrorMessage);
    }

    [TestMethod]
    public void ValorNulo_NoDevuelveError()
    {
        // preparación
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); 
        string valor = null;
        var valContext = new ValidationContext( new {Nombre = valor});

        // Ejecución
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificación
        Assert.IsNull(resultado);
    }

    
    [TestMethod]
    public void ValorConPrimeraLetraMinuscula_NoDevuelveError()
    {
        // preparación
        var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute(); 
        var valor = "Felipe";
        var valContext = new ValidationContext( new {Nombre = valor});

        // Ejecución
        var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

        // Verificación
        Assert.IsNull(resultado);
    }
}