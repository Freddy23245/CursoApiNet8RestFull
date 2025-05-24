using System.ComponentModel.DataAnnotations;

namespace API_Pelicula.Models.Dtos
{
    public class CrearCategoriaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(60, ErrorMessage = "El numero de Maximo caracteres no tiene que superar a los 60")]
        public string NombreCategoria { get; set; }
    }
}
