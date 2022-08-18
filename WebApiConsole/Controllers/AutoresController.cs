using System;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Controllers.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApiAutores.Filtros;
using WebApiAutores.DTOs;
using AutoMapper;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper){
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet] // api/autores => (la toma por defecto la ruta)
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        // ejemplo con una variable
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var autor =  await context.Autores.FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return mapper.Map<AutorDTO>(autor);
        }
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores =  await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }
// ------------------------------------
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO){
            var existeAutorMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (existeAutorMismoNombre)
            {
                return BadRequest($"Ya existe el mismo autor con el mismo nombre {autorCreacionDTO.Nombre}");
            }
            
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]// [HttpPut("{id:int}")] se establece o complementa la ruta definida arriba api/autores/id 
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de url");                
            }

            var existe = await context.Autores.AnyAsync( x => x.Id == id); // Valida si exiaste el id
            if (!existe)
            {
                return NotFound();                
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]// [HttpDelete("{id:int}")] se establece o complementa la ruta definida arriba api/autores/id 
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync( x => x.Id == id); // Valida si exiaste el id
            if (!existe)
            {
                return NotFound();                
            }

            context.Remove(new Autor() {Id = id});// se instancia el objeto tipo Autor para ser removido por que el entity framework lo requiere / no se crea uno nuevo
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

