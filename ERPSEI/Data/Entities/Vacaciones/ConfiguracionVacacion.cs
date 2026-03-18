using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Vacaciones
{
    [Table("ConfiguracionesVacaciones")]
    public class ConfiguracionVacacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoVisualizacion { get; set; } = "LegalesProporcionales";
        // Valores:
        // "Legales"
        // "LegalesProporcionales"

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}