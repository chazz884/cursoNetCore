using System;
using AutoMapper;
using WebApiAutores.Controllers.Entidades;
using WebApiAutores.DTOs;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<LibroCreacionDTO, Libro>().ForMember(libro => libro.AutorLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<ComentarioCreacionDTO, Comentario>();//post
            CreateMap<Comentario, ComentarioDTO>();//get
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (libroCreacionDTO.AutoresId == null) {return resultado;}

            foreach (var autorId in libroCreacionDTO.AutoresId)
            {
                resultado.Add(new AutorLibro(){AutorId = autorId});
            }

            return resultado;
        }
    }
}

