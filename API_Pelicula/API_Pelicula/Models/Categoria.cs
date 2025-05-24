using System.ComponentModel.DataAnnotations;

namespace API_Pelicula.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }
        [Required]
        public string NombreCategoria { get; set; }
        [Required]
        public  DateTime FechaCreacion { get; set; }

    }
}
