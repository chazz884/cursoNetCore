using System;
using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilidades
{
    // se usa una clase estatica cuando usamos un metodo de extension, tampoco puede llevar un constructor
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}

