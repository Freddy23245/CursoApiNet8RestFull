using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using AutoMapper;

namespace API_Pelicula.PeliculaMappers
{
    public class PeliculasMapper:Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria,CategoriaDto>().ReverseMap();
            CreateMap<Categoria,CrearCategoriaDto>().ReverseMap();

            CreateMap<Peliculas, PeliculasDto>().ReverseMap();
            CreateMap<Peliculas, CrearPeliculaDto>().ReverseMap();
        }
    }
}
