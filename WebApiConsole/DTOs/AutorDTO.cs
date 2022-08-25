using System;
namespace WebApiAutores.DTOs
{
    public class AutorDTO : Recurso
    {
        public AutorDTO()
        {
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}

