using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;

namespace API_Pelicula.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<Usuarios> GetUsuarios();
        Usuarios GetUsuariosId(int id);
        bool IsUniqueUser(string usuarioNombre);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuario);
        Task<Usuarios> Registro(UsuarioRegistroDto usuario);
        bool Guardar();
    }
}
