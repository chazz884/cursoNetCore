using System;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers.Entidades;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            
            // var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id); 
            // var libro = await context.Libros.Include(LibroDB => LibroDB.Comentarios).FirstOrDefaultAsync(x => x.Id == id); relacion 1  muchos

            var libro = await context.Libros.Include(libroDB => libroDB.AutorLibros).ThenInclude(autorLibro => autorLibro.Autor)
            .FirstOrDefaultAsync(x => x.Id == id);// con es te query estamos trayyendo daata de los autores de cada libro

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutorLibros = libro.AutorLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            //query
            // var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            // if (!existeAutor)
            // {
            //     return BadRequest($"No existe el autor de id : {libro.Id}");
            // }

            if (libroCreacionDTO.AutoresId == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores
            .Where(autorBD => libroCreacionDTO.AutoresId.Contains(autorBD.Id)).Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresId.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new {id = libro.Id}, libroDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutorLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();
            return NoContent();
        
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutorLibros != null)
            {
                for (int i = 0; i < libro.AutorLibros.Count; i++)
                {
                    libro.AutorLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroBD = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if(libroBD == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroBD);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroBD);
            
            await context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "borrarLibro")]// [HttpDelete("{id:int}")] se establece o complementa la ruta definida arriba api/autores/id 
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync( x => x.Id == id); // Valida si exiaste el id
            if (!existe)
            {
                return NotFound();                
            }

            context.Remove(new Libro() {Id = id});// se instancia el objeto tipo Autor para ser removido por que el entity framework lo requiere / no se crea uno nuevo
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}

