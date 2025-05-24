using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API_Pelicula.Controllers
{
    //[Authorize(Roles ="Admin")] bloquea todos los metodos de la clase
    //[ResponseCache(Duration =20)]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _catRepositorio;
        private readonly IMapper _mapper;
        public CategoriasController(ICategoriaRepositorio categoriaRepositorio, IMapper mapper)
        {
            _catRepositorio = categoriaRepositorio;
            _mapper = mapper;
        }
        //[AllowAnonymous] Sirve par aponer el metodo sin autenticarse(publico)
        [HttpGet]
        //[ResponseCache(Duration =20)]// guarda la consulta en cache por 20 segundos
        //esto lo que hace es decirle que no guarde en cache se usa para metodos que actualizan datos recurrentemente
        //[ResponseCache(Location = ResponseCacheLocation.None,NoStore =true)]
        // con lo agregado el perfil global no tengo que usar esto  [ResponseCache(Duration =20)] solo llamo por el nombre
        //y lo tendria concentrado todo en un solo lugar
        [ResponseCache(CacheProfileName = "PorDefecto20Segundos")] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias() 
        { 
            var ListaCategorias = _catRepositorio.GetCategorias();
            var ListaCategoriaDto = new List<CategoriaDto>();
            foreach (var item in ListaCategorias) {

                ListaCategoriaDto.Add(_mapper.Map<CategoriaDto>(item)); 
            }
            return Ok(ListaCategoriaDto);
        }

        [HttpGet("{id:int}",Name ="GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int id)
        {

            var ListaCategorias = _catRepositorio.GetCategoriaId(id);
            if (ListaCategorias == null)
                return NotFound();
            var ListaCategoriaDto = new List<CategoriaDto>();
            
            ListaCategoriaDto.Add(_mapper.Map<CategoriaDto>(ListaCategorias));
        
            return Ok();
        }
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto categoria)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(categoria == null)
            {
                return BadRequest(ModelState);
            }
            if (_catRepositorio.ExisteCategoriaPorNombre(categoria.NombreCategoria))
            {
                 ModelState.AddModelError("","La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var categoriaNueva = _mapper.Map<Categoria>(categoria);
            if(!_catRepositorio.CrearCategoria(categoriaNueva))
            {
                ModelState.AddModelError("", "No se pudo crear la Categoria.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria",new {id = categoriaNueva.IdCategoria},categoriaNueva);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ActualizarPatchCategoria(int catId,[FromBody] CategoriaDto categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoria == null || catId != categoria.IdCategoria)
            {
                return BadRequest(ModelState);
            }

            var categoriaNueva = _mapper.Map<Categoria>(categoria);
            if (!_catRepositorio.ActualizarCategoria(categoriaNueva))
            {
                ModelState.AddModelError("", "No se pudo Actualizar la Categoria.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ActualizarPutCategoria(int catId, [FromBody] CategoriaDto categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoria == null || catId != categoria.IdCategoria)
            {
                return BadRequest(ModelState);
            }
            var catExiste = _catRepositorio.GetCategoriaId(catId);
            if (catExiste == null) return NotFound($"no se encontro la categoria con el id {catId}");
            var categoriaNueva = _mapper.Map<Categoria>(categoria);
            if (!_catRepositorio.ActualizarCategoria(categoriaNueva))
            {
                ModelState.AddModelError("", "No se pudo Actualizar la Categoria.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCategoria(int id)
        {
            if (!_catRepositorio.ExisteCategoriaPorId(id))
                return NotFound("La Categoria no existe");
            var categ = _catRepositorio.GetCategoriaId(id);
            if (!_catRepositorio.BorrarCategoria(categ))
            {
                ModelState.AddModelError("", "No se pudo Eliminar la Categoria.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}


