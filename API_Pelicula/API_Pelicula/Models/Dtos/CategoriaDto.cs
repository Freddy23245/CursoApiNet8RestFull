using System.ComponentModel.DataAnnotations;

namespace API_Pelicula.Models.Dtos
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        [MaxLength(60,ErrorMessage ="El numero de Maximo caracteres no tiene que superar a los 60")]
        public string NombreCategoria { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
