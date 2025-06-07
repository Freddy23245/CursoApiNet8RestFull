using Microsoft.AspNetCore.Identity;

namespace API_Pelicula.Models
{
    public class AppUsuarios:IdentityUser
    {
        //la tabla generada por el entity ya viene con campos pero aca
        //le podes agregar mas campos ya que  hereda los campos del identity
        public string Nombre { get; set; }
    }
}
