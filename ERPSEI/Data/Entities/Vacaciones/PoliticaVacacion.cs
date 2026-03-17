using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Vacaciones
{
    [Table("PoliticasVacaciones")]
    public class PoliticaVacacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty; // Ej. Legal 2023

        [Required]
        [StringLength(50)]
        public string TipoVacacion { get; set; } = string.Empty; // Legales / Anuales

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<PoliticaVacacionDetalle> Detalles { get; set; } = new List<PoliticaVacacionDetalle>();
    }
}