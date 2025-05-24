using API_Pelicula.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Pelicula.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }

        //Aqui pasar todas las entidades
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Peliculas> Peliculas { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
    }
}
