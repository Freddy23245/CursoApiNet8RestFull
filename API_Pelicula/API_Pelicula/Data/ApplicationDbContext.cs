using API_Pelicula.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Pelicula.Data
{
    public class ApplicationDbContext:IdentityDbContext<AppUsuarios>//DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        //Aqui pasar todas las entidades
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Peliculas> Peliculas { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<AppUsuarios> AppUsuarios { get; set; }
    }
}
