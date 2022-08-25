using System;
namespace WebApiAutores.DTOs
{
    public class DatoHATEOAS
    {
        public string Enlace { get; set; }
        public string Descripcion { get; set; }
        public string Metodo { get; set; }

        public DatoHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }
    }
}

