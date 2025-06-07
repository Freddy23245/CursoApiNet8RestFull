using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;

namespace API_Pelicula.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<AppUsuarios> GetUsuarios();
        AppUsuarios GetUsuariosId(string id);
        bool IsUniqueUser(string usuarioNombre);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuario);
        Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuario);
        bool Guardar();
    }
}
