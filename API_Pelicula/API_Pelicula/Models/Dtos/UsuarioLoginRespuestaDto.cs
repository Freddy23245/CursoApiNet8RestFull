using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API_Pelicula.Models.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public Usuarios usuario {  get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

    }
}
