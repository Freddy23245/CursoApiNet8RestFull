using API_Pelicula.Models;
using API_Pelicula.Models.Dtos;
using API_Pelicula.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Pelicula.Controllers
{
    [Authorize(Roles = "Administrador")]
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
        //V1
        //[AllowAnonymous]
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public IActionResult GetPeliculas()
        //{

        //    var ListaPeliculas = _pelRepositorio.GetPeliculas();
        //    var ListaPeliculasDto = new List<PeliculasDto>();
        //    foreach (var item in ListaPeliculas)
        //    {

        //        ListaPeliculasDto.Add(_mapper.Map<PeliculasDto>(item));
        //    }
        //    return Ok(ListaPeliculasDto);
        //}

        //V2 Paginacion
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2)
        {
            try
            {
                var totalPeliculas = _pelRepositorio.GetTotalPeliculas();
                var ListaPeliculas = _pelRepositorio.GetPeliculas(pageNumber, pageSize);

                if(ListaPeliculas == null || !ListaPeliculas.Any())
                {
                    return NotFound("No se encontraron Peliculas.");
                }
                var peliculasDto = ListaPeliculas.Select(p => _mapper.Map<PeliculasDto>(p)).ToList();

                var response = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalPeliculas / (double)pageSize),
                    TotalItems = totalPeliculas,
                    Items = peliculasDto
                };

                return Ok(response);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener datos de la aplicacion");
            }
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
        public IActionResult CrearCategoria([FromForm] CrearPeliculaDto pelicula)
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
            //if (!_pelRepositorio.CrearPelicula(peliculaNueva))
            //{
            //    ModelState.AddModelError("", "No se pudo crear la pelicula.");
            //    return StatusCode(500, ModelState);
            //}
            //Subida de Archivo
            if (pelicula.Imagen != null)
            {
                string NombreArchivo = peliculaNueva.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(pelicula.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + NombreArchivo;
                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);
                FileInfo file = new FileInfo(ubicacionDirectorio);

                if (file.Exists)
                {
                    file.Delete();
                }
                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    //con esto enviamos el archivo a la carpeta creada anteriormente
                    pelicula.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                peliculaNueva.RutaImagen = baseUrl + "/ImagenesPeliculas/"+NombreArchivo;
                peliculaNueva.RutalocalImagen = rutaArchivo;
            }
            else
            {
                peliculaNueva.RutaImagen = "https://placehold.co/600x400";
            }
            _pelRepositorio.CrearPelicula(peliculaNueva);
            return CreatedAtRoute("GetPelicula", new { id = peliculaNueva.Id }, peliculaNueva);
        }
        [HttpPut("ActualizarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ActualizarPutPelicula([FromForm] ActualizarPeliculaDto pelicula)
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
            //if (!_pelRepositorio.ActualizarPelicula(peliNueva))
            //{
            //    ModelState.AddModelError("", "No se pudo Actualizar la Pelicula.");
            //    return StatusCode(500, ModelState);
            //}
            if (pelicula.Imagen != null)
            {
                string NombreArchivo = peliNueva.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(pelicula.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + NombreArchivo;
                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);
                FileInfo file = new FileInfo(ubicacionDirectorio);

                if (file.Exists)
                {
                    file.Delete();
                }
                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    //con esto enviamos el archivo a la carpeta creada anteriormente
                    pelicula.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                peliNueva.RutaImagen = baseUrl + "/ImagenesPeliculas/" + NombreArchivo;
                peliNueva.RutalocalImagen = rutaArchivo;
            }
            else
            {
                peliNueva.RutaImagen = "https://placehold.co/600x400";
            }
            _pelRepositorio.ActualizarPelicula(peliNueva);
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
            try
            {
                var listaPeliculas = _pelRepositorio.GetPeliculasEnCategorias(idCategoria);
                if (listaPeliculas == null || !listaPeliculas.Any())
                    return NotFound($"No se Encontraron peliculas con el id de categoria {idCategoria}");
                var itemPeliculas = listaPeliculas.Select(pelicula => _mapper.Map<PeliculasDto>(pelicula)).ToList();
                //foreach (var item in listaPeliculas)
                //{
                //    itemPeliculas.Add(_mapper.Map<PeliculasDto>(item));

                //}
                return Ok(itemPeliculas);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error en recuperar datos de la aplicacion");
            }
        }
        [HttpGet("Buscar")]
        public IActionResult BuscarPeliculas(string nombre)
        {
            try
            {
                var listaPeliculas = _pelRepositorio.BuscarPeliculas(nombre);
                if (!listaPeliculas.Any())
                    return NotFound($"No se Encontraron peliculas con el id de categoria {nombre}");
                var peliculasDto = _mapper.Map<IEnumerable<PeliculasDto>>(listaPeliculas);
                return Ok(peliculasDto);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la lista");
            }
        }
    }
}
