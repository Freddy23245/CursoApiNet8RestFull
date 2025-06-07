using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Pelicula.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/Usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        public UsuariosController(IUsuarioRepositorio usuarioRepositorio,IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetUsuarios()
        {

            var ListaUsuarios = _usuarioRepositorio.GetUsuarios();
            var ListaUsuariosDto = new List<UsuarioDto>();
            foreach (var item in ListaUsuarios)
            {

                ListaUsuariosDto.Add(_mapper.Map<UsuarioDto>(item));
            }
            return Ok(ListaUsuariosDto);
        }
        [HttpGet( "GetUsuario")]//"{id:int}", Name =
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuarioId(string id)
        {

            var ListaUsuario = _usuarioRepositorio.GetUsuariosId(id);
            if (ListaUsuario == null)
                return NotFound();
            var ListaUsuarioDto = new List<UsuarioDto>();

            ListaUsuarioDto.Add(_mapper.Map<UsuarioDto>(ListaUsuario));

            return Ok(ListaUsuarioDto);
        }
        [AllowAnonymous]
        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto user)
        {
            if(user == null)
                return BadRequest();
            bool validarExistencia = _usuarioRepositorio.IsUniqueUser(user.NombreUsuario);
            if (validarExistencia) {
                return BadRequest("Ya existe el nombre de usuario");
            }

           await  _usuarioRepositorio.Registro(user);

            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto user)
        {
            var userLogin = await _usuarioRepositorio.Login(user);
            if(userLogin.usuario == null || string.IsNullOrEmpty(userLogin.Token))
            {
                return BadRequest("usuario o contraseña incorrectos");
            }
            return Ok(userLogin);
        }
    }
}
