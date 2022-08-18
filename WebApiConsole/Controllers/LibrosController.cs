using System;
using AutoMapper;
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id); 
            // var libro = await context.Libros.Include(LibroDB => LibroDB.Comentarios).FirstOrDefaultAsync(x => x.Id == id); relacion 1  muchos
            return mapper.Map<LibroDTO>(libro);
        }

        [HttpPost]
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

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}

