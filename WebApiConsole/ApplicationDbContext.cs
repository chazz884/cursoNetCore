using System;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // El dbcontext lo que hace es armar la estructura o esquema de la base de datos
        // con las entidades o modelos definidos 

        public DbSet <Autor> Autores { get; set; }
        // se pone explicitamente libros para poder hacer directmanete querys 
        public DbSet <Libro> Libros { get; set; }
    }
}

