using API_Pelicula.Models;

namespace API_Pelicula.Repositorio.IRepositorio
{
    public interface IPeliculasRepositorio
    {
        ICollection<Peliculas> GetPeliculas();
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
