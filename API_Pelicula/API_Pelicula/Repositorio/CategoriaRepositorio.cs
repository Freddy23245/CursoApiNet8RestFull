using API_Pelicula.Data;
using API_Pelicula.Models;
using API_Pelicula.Repositorio.IRepositorio;

namespace API_Pelicula.Repositorio
{
    public class CategoriaRepositorio :ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _context;
        public CategoriaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            var categoriaExistente = _context.Categoria.Find(categoria.IdCategoria);
            if (categoriaExistente != null)
            {
                _context.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _context.Categoria.Update(categoria);
            }

                return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _context.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _context.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoriaPorId(int id)
        {
            return _context.Categoria.Any(c => c.IdCategoria == id);
        }

        public bool ExisteCategoriaPorNombre(string nombre)
        {
             var Valor = _context.Categoria.Any(c => c.NombreCategoria.ToLower().Trim() == nombre.ToLower().Trim());
            return Valor;
        }

        public Categoria GetCategoriaId(int id)
        {
            return _context.Categoria.FirstOrDefault(c => c.IdCategoria == id);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _context.Categoria.OrderBy(c => c.NombreCategoria).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges()  >=0 ? true : false;
        }
    }
}
