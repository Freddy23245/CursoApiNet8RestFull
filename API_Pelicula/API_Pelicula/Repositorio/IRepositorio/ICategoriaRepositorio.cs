using API_Pelicula.Models;

namespace API_Pelicula.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio
    {
        ICollection<Categoria>GetCategorias();
        Categoria GetCategoriaId(int id);
        bool ExisteCategoriaPorId(int id);
        bool ExisteCategoriaPorNombre(string nombre);

        bool CrearCategoria(Categoria categoria);
        bool ActualizarCategoria(Categoria categoria);
        bool BorrarCategoria(Categoria categoria);
        bool Guardar();
    }
}
