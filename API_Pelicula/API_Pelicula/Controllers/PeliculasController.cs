using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Pelicula.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculasRepositorio _pelRepositorio;
        private readonly IMapper _mapper;
        public PeliculasController(IPeliculasRepositorio pelRepositorio, IMapper mapper)
        {
            _pelRepositorio = pelRepositorio;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {

            var ListaPeliculas = _pelRepositorio.GetPeliculas();
            var ListaPeliculasDto = new List<PeliculasDto>();
            foreach (var item in ListaPeliculas)
            {

                ListaPeliculasDto.Add(_mapper.Map<PeliculasDto>(item));
            }
            return Ok(ListaPeliculasDto);
        }
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int id)
        {

            var ItemPelicula = _pelRepositorio.GetPeliculaId(id);
            if (ItemPelicula == null)
                return NotFound();
            var ListaPeliculaDto = new List<PeliculasDto>();

            ListaPeliculaDto.Add(_mapper.Map<PeliculasDto>(ItemPelicula));

            return Ok();
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearCategoria([FromBody] CrearPeliculaDto pelicula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (pelicula == null)
            {
                return BadRequest(ModelState);
            }
            if (_pelRepositorio.ExistePeliculaPorNombre(pelicula.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var peliculaNueva = _mapper.Map<Peliculas>(pelicula);
            if (!_pelRepositorio.CrearPelicula(peliculaNueva))
            {
                ModelState.AddModelError("", "No se pudo crear la pelicula.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetPelicula", new { id = peliculaNueva.Id }, peliculaNueva);
        }
        [HttpPut("ActualizarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ActualizarPutPelicula([FromBody] PeliculasDto pelicula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (pelicula == null)
            {
                return BadRequest(ModelState);
            }
            var catExiste = _pelRepositorio.GetPeliculaId(pelicula.Id);
            if (catExiste == null) return NotFound($"no se encontro la pelicula con el id {pelicula.Id}");
            var peliNueva = _mapper.Map<Peliculas>(pelicula);
            if (!_pelRepositorio.ActualizarPelicula(peliNueva))
            {
                ModelState.AddModelError("", "No se pudo Actualizar la Pelicula.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePelicula(int id)
        {
            if (!_pelRepositorio.ExistePeliculaPorId(id))
                return NotFound("La Pelicula no existe");
            var pel = _pelRepositorio.GetPeliculaId(id);
            if (!_pelRepositorio.BorrarPelicula(pel))
            {
                ModelState.AddModelError("", "No se pudo Eliminar la pelicula.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpGet("PeliculaEnCategoria")]
        public IActionResult PeliculaEnCategoria(int idCategoria)
        {
            var listaPeliculas = _pelRepositorio.GetPeliculasEnCategorias(idCategoria);
            if (listaPeliculas == null)
                return NotFound();
            var itemPeliculas = new List<PeliculasDto>();
            foreach (var item in listaPeliculas)
            {
                itemPeliculas.Add(_mapper.Map<PeliculasDto>(item));

            }
            return Ok(itemPeliculas);
        }
        [HttpGet("Buscar")]
        public IActionResult BuscarPeliculas(string nombre)
        {
            try
            {
                var listaPeliculas = _pelRepositorio.BuscarPeliculas(nombre);
                if (listaPeliculas.Any())
                    return Ok(listaPeliculas);
                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la lista");
            }
        }
    }
}
