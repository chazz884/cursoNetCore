using System;
namespace WebApiAutores.DTOs
{
    public class Recurso
    {
        public Recurso()
        {
        }

        public List<DatoHATEOAS> Enlaces { get; set; } = new List<DatoHATEOAS>();
    }
}

