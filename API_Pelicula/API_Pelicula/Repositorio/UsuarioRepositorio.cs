using API_Pelicula.Data;
using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUsuarios> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UsuarioRepositorio(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUsuarios> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _claveSecreta = configuration.GetValue<string>("ApiSettings:Secreta");
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public ICollection<AppUsuarios> GetUsuarios()
        {
            return _context.AppUsuarios.ToList();
        }

        public AppUsuarios GetUsuariosId(string id)
        {
            return _context.AppUsuarios.FirstOrDefault(c => c.Id == id);
        }

        public bool Guardar()
        {
            throw new NotImplementedException();
        }

        public bool IsUniqueUser(string usuarioNombre)
        {
            var existe = _context.AppUsuarios.Any(c => c.UserName == usuarioNombre);
            return existe;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuario)
        {
            //var passwordEncriptada = ObtenerMD5(usuario.Password);
            var usuarioLogueado = _context.AppUsuarios.FirstOrDefault(
                c => c.UserName.ToLower() == usuario.NombreUsuario.ToLower());

            //metodo propio de identity que verifica los password wooow
            var isBool = await _userManager.CheckPasswordAsync(usuarioLogueado,usuario.Password);

            if (usuarioLogueado == null || isBool == false)
            { return new UsuarioLoginRespuestaDto() { Token = "", usuario = null, Role = null }; }

            var roles = await _userManager.GetRolesAsync(usuarioLogueado);
                var manejoToken = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_claveSecreta);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,usuarioLogueado.UserName.ToString()),
                        new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
                };

            var Token = manejoToken.CreateToken(tokenDescription);
            UsuarioLoginRespuestaDto response = new UsuarioLoginRespuestaDto()
            {
                Token = manejoToken.WriteToken(Token),
                usuario = _mapper.Map<UsuarioDatosDto>(usuarioLogueado),
               //Role = roles.Select(x=>x.)
                
            };
            return response;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuario)
        {
            //el encriptado ya lo hace identity
            //var passwordEncriptada = ObtenerMD5(usuario.Password);

            AppUsuarios userNuevo = new AppUsuarios()
            {
                Nombre = usuario.Nombre,
                UserName = usuario.NombreUsuario,
                Email = usuario.NombreUsuario,
                NormalizedEmail = usuario.NombreUsuario.ToUpper()
            };

            var result = await _userManager.CreateAsync(userNuevo,usuario.Password);
            if(result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Administrador").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Administrador"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(userNuevo, "Administrador");
                var usuarioDevuelto = _context.AppUsuarios.FirstOrDefault(u=>u.UserName== usuario.NombreUsuario);

                return _mapper.Map<UsuarioDatosDto>(usuarioDevuelto);
            }

            //_context.Usuarios.Add(userNuevo);
            //await _context.SaveChangesAsync();
            //userNuevo.Password = passwordEncriptada;
            return new UsuarioDatosDto();
        }
        //private string ObtenerMD5(string valor)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        //    data = x.ComputeHash(data);
        //    string resp = "";
        //    for (int i = 0; i < data.Length; i++)
        //        resp += data[i].ToString("x2").ToLower();
        //    return resp;

        //}
    }
}
