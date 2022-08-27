using System;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Controllers.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApiAutores.Filtros;
using WebApiAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Utilidades;

//Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImNhcmxvc0Bob3RtYWlsLmNvbSIsImxvIHF1ZSB5byBxdWllcmEiOiJvdHJvIHZhbG9yIiwiZXNBZG1pbiI6IjEiLCJleHAiOjE2OTI5MTc2Njl9.wtnZr2nLiQoYVgbPl2dY7jOTd9f3dBmWZZxYlOjVbAQ

namespace WebApiAutores.Controllers.V2
{
    [ApiController]
    [Route("api/autores")]
    // [Route("api/v2/autores")]
    [CabeceraEstaPresenteAtributte("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;
        // private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration,
        IAuthorizationService authorizationService){
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
            // this.configuration = configuration;
        }
        // usando el appseting, apuntar a diferentes ambientes desde cs
        // [HttpGet("configuraciones")]
        // public ActionResult<string> ObtenerConfiguracion()
        // {
        //     return configuration["apellido"];
        // }

        [HttpGet(Name = "obtenerAutoresv2")] // api/autores => (la toma por defecto la ruta)
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autores);

            // if (incluirHATEOAS)
            // {
            //     var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            //     // dtos.ForEach(dto => GenerarEnlaces(dto, esAdmin.Succeeded));

            //     var resultado = new ColeccionDeRecursos<AutorDTO> {Valores = dtos};
            //     resultado.Enlaces.Add(
            //         new DatoHATEOAS(
            //             enlace: Url.Link("obtenerAutores",
            //             new {}
            //         ),
            //         descripcion: "self",
            //         metodo: "GET"
            //     ));
            //     if (esAdmin.Succeeded)
            //     {
            //         resultado.Enlaces.Add(
            //             new DatoHATEOAS(
            //                 enlace: Url.Link("crearAutor",
            //                 new {}
            //             ),
            //             descripcion: "crear-autor",
            //             metodo: "POST"
            //         ));
            //     }

            //     return Ok(resultado);
            // }
            // return Ok(dtos);
        }

        // ejemplo con una variable
        [HttpGet("{id:int}", Name = "obtenerAutorv2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            // var autor =  await context.Autores.FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            var autor = await context.Autores
            .Include(autorDB => autorDB.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Libro)
            .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);

            return dto;
        }
        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev2")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autores =  await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }
// ------------------------------------
        [HttpPost(Name = "crearAutorv2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO){
            var existeAutorMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (existeAutorMismoNombre)
            {
                return BadRequest($"Ya existe el mismo autor con el mismo nombre {autorCreacionDTO.Nombre}");
            }
            
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            
            // Para buenas practicas se retorna donde se encuentra la data :: 
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new {id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}",Name = "actualizarAutorv2")]// [HttpPut("{id:int}")] se establece o complementa la ruta definida arriba api/autores/id 
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync( x => x.Id == id); // Valida si exiaste el id
            if (!existe)
            {
                return NotFound();                
            }
            
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutorv2")]// [HttpDelete("{id:int}")] se establece o complementa la ruta definida arriba api/autores/id 
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

