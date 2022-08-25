using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers.Entidades;

namespace WebApiAutores
{
    // public class ApplicationDbContext: DbContext
    public class ApplicationDbContext: IdentityDbContext 
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // El dbcontext lo que hace es armar la estructura o esquema de la base de datos
        // con las entidades o modelos definidos 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutorLibro>().HasKey(al => new {al.AutorId, al.LibroId});
        }

        public DbSet <Autor> Autores { get; set; }
        // se pone explicitamente entidad para poder hacer directmanete querys 
        public DbSet <Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}

