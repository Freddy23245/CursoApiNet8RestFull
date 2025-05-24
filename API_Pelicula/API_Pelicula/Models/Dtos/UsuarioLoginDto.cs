using System.ComponentModel.DataAnnotations;

namespace API_Pelicula.Models.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "Se requiere que ingese el nombre de usuario.")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Se requiere que ingese la password.")]
        public string Password { get; set; }
    }
}
