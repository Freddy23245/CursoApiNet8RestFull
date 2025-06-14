using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Pelicula.Models
{
    public class Peliculas
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string? RutaImagen { get; set; }
        public string? RutalocalImagen { get; set; }
        public enum TipoClasificacion
        {
            Siete,Trece,Dieciseis,Dieciocho
        } 
        public TipoClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        //Relacion con tabla Categoria o Modelo
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
    }
}
