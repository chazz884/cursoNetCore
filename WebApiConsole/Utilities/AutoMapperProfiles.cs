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
            CreateMap<AutorCreacionDTO, Autor>(); // post
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
            .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros)); // get

            CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(libro => libro.AutorLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
            .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            CreateMap<ComentarioCreacionDTO, Comentario>();//post
            CreateMap<Comentario, ComentarioDTO>();//get
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO(){
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if (libro.AutorLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in libro.AutorLibros)
            {
                resultado.Add(new AutorDTO(){
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
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

