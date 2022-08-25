using System;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Controllers.Entidades;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Servicios;
using Microsoft.AspNetCore.Authorization;
using WebApiAutores.Filtros;
// En el controlodaro se hacen los llamados o peticiones para el crud
namespace WebApiAutores.Controllers
{
    [ApiController]//Decorador con atributo que permite hacer validaciones automaticas
    [Route("api/autores")]  // cuando se reemplaza la url con [controller] toma el nombre del controlodor p, esto es si en un futuro cambia es como una variable
    // los decoradores son con []
    // [Authorize] se protegen todas las peticiones a nivel de controlodor
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScope servicioScope;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioTransient servicioTransient, ServicioScope servicioScope, 
            ServicioSingleton servicioSingleton, ILogger<AutoresController> logger){
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScope = servicioScope;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        // Lo genial de ResponseCache es que la info de respuesta se almacena en caché durante x tiempo para que la siguiente peticion la consume de caché
        // [ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObptenerGuids()
        {
            return Ok(new {
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),

                AutoresController_Scoped = servicioScope.Guid,
                ServicioA_Scope = servicio.ObtenerScope(),
                
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton()
            });
        }

        // las rutas se pueden definir de la siguiente manera (aplica para todo el crud) y se pueden reemplazar con varibales para un end point específico
        [HttpGet] // api/autores => (la toma por defecto la ruta)
        [HttpGet("listado")] // api/autores/listado => (se agrega ruta específica)
        [HttpGet("/listado")] // listado => (se reemplaza la base api/autores y se llama directamente listado)
        // [Authorize]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            throw new NotImplementedException();
            logger.LogInformation("Obteniendo listado autores");
            logger.LogWarning("Obteniendo listado autores test");
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int valor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        // ejemplo con una variable
        [HttpGet("{id:int}/{param2}")]
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromRoute] string nombre)
        {
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Nombre == nombre);

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }
        // puede funcionar con llamados sincronos el async se usa si consumimos apis externos  casi siempre
        [HttpGet("sincrona")]
        public List<Autor> GetSincrona()
        {
            return context.Autores.Include(x => x.Libros).ToList();
        }
// ------------------------------------
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor){
            var existeAutorMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if (existeAutorMismoNombre)
            {
                return BadRequest($"Ya existe el mismo autor con el mismo nombre {autor.Nombre}");
            }
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

