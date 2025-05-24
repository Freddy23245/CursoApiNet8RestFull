using API_Pelicula.Data;
using API_Pelicula.Models;
using API_Pelicula.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace API_Pelicula.Repositorio
{
    public class PeliculasRepositorio:IPeliculasRepositorio
    {
        private readonly ApplicationDbContext _context;
        public PeliculasRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarPelicula(Peliculas Peliculas)
        {
            Peliculas.FechaCreacion = DateTime.Now;
            var peliculaExistente = _context.Peliculas.Find(Peliculas.Id);
            if (peliculaExistente != null)
            {
                _context.Entry(peliculaExistente).CurrentValues.SetValues(Peliculas);
            }
            else
            {
                _context.Peliculas.Update(Peliculas);
            }
            return Guardar();
        }

        public bool BorrarPelicula(Peliculas Peliculas)
        {
            _context.Peliculas.Remove(Peliculas);
            return Guardar();
        }

        public bool CrearPelicula(Peliculas Peliculas)
        {
            Peliculas.FechaCreacion = DateTime.Now;
            _context.Peliculas.Add(Peliculas);
            return Guardar();
        }

        public bool ExistePeliculaPorId(int id)
        {
            return _context.Peliculas.Any(c => c.Id == id);
        }

        public bool ExistePeliculaPorNombre(string nombre)
        {
            var Valor = _context.Peliculas.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return Valor;
        }

        public Peliculas GetPeliculaId(int id)
        {
            return _context.Peliculas.FirstOrDefault(c => c.Id == id);
        }

        public ICollection<Peliculas> GetPeliculas()
        {
            return _context.Peliculas.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
        public ICollection<Peliculas> GetPeliculasEnCategorias(int IdCategoria)
        {
            return _context.Peliculas.Include(ca=>ca.Categoria).Where(ca=>ca.CategoriaId == IdCategoria).ToList();
        }

        public IEnumerable<Peliculas> BuscarPeliculas(string nombre)
        {
            IQueryable<Peliculas> query = _context.Peliculas;
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

    }
}
