using API_Pelicula.Models;

namespace API_Pelicula.Repositorio.IRepositorio
{
    public interface IPeliculasRepositorio
    {
        //V1
        //ICollection<Peliculas> GetPeliculas();
        //V2
        ICollection<Peliculas> GetPeliculas(int PageNumber,int PageSize);
        int GetTotalPeliculas();
        ICollection<Peliculas> GetPeliculasEnCategorias(int IdCategoria);
        IEnumerable<Peliculas> BuscarPeliculas(string nombre);
        Peliculas GetPeliculaId(int id);
        bool ExistePeliculaPorId(int id);
        bool ExistePeliculaPorNombre(string nombre);
        bool CrearPelicula(Peliculas pelicula);
        bool ActualizarPelicula(Peliculas pelicula);
        bool BorrarPelicula(Peliculas pelicula);
        bool Guardar();
    }
}
