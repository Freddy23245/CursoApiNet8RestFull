using API_Pelicula.Data;
using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace API_Pelicula.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        private string _claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _claveSecreta = configuration.GetValue<string>("ApiSettings:Secreta");
        }
        public ICollection<Usuarios> GetUsuarios()
        {
            return _context.Usuarios.ToList();
        }

        public Usuarios GetUsuariosId(int id)
        {
            return _context.Usuarios.FirstOrDefault(c => c.Id == id);
        }

        public bool Guardar()
        {
            throw new NotImplementedException();
        }

        public bool IsUniqueUser(string usuarioNombre)
        {
            var existe = _context.Usuarios.Any(c => c.NombreUsuario == usuarioNombre);
            return existe;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuario)
        {
            var passwordEncriptada = ObtenerMD5(usuario.Password);
            var usuarioLogueado = _context.Usuarios.FirstOrDefault(
                c => c.NombreUsuario.ToLower() == usuario.NombreUsuario.ToLower()
                && c.Password == passwordEncriptada
                );
            if (usuarioLogueado == null)
                return new UsuarioLoginRespuestaDto() { Token = "", usuario = null, Role = null };

                var manejoToken = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_claveSecreta);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,usuarioLogueado.NombreUsuario.ToString()),
                        new Claim(ClaimTypes.Role,usuarioLogueado.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
                };

            var Token = manejoToken.CreateToken(tokenDescription);
            UsuarioLoginRespuestaDto response = new UsuarioLoginRespuestaDto()
            {
                Token = manejoToken.WriteToken(Token),
                usuario = usuarioLogueado,
                Role = usuarioLogueado.Role
                
            };
            response.usuario.Password = null;
            return response;
        }

        public async Task<Usuarios> Registro(UsuarioRegistroDto usuario)
        {
            var passwordEncriptada = ObtenerMD5(usuario.Password);

            Usuarios userNuevo = new Usuarios()
            {
                Nombre = usuario.Nombre,
                NombreUsuario = usuario.NombreUsuario,
                Password = passwordEncriptada,
                Role = usuario.Role
            };
            _context.Usuarios.Add(userNuevo);
            await _context.SaveChangesAsync();
            userNuevo.Password = passwordEncriptada;
            return userNuevo;
        }
        private string ObtenerMD5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;

        }
    }
}
