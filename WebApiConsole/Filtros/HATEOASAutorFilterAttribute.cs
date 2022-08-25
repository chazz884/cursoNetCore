using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Filtros
{
    public class HATEOASAutorFilterAttribute: HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;

            var autorDTO = resultado.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ?? throw new ArgumentException("Se esperaba una instancia de autordto o list");

                autoresDTO.ForEach(async autor => await generadorEnlaces.GenerarEnlaces(autor));

                resultado.Value = autoresDTO;

            } else
            {
                await generadorEnlaces.GenerarEnlaces(autorDTO);
            }

            // var modelo = resultado.Value as AutorDTO ?? throw new ArgumentException("Se esperaba una instancia de autordto");

            await next();
        } 
    }
}
