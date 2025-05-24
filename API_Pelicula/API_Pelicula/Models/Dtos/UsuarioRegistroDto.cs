using System.ComponentModel.DataAnnotations;

namespace API_Pelicula.Models.Dtos
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage ="Se requiere que ingese el nombre de usuario.")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Se requiere que ingese el nombre.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere que ingese la password.")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
