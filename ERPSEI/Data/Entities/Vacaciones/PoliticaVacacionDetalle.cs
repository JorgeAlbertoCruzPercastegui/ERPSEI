using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Vacaciones
{
    [Table("PoliticasVacacionesDetalles")]
    public class PoliticaVacacionDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PoliticaVacacionId { get; set; }

        [ForeignKey(nameof(PoliticaVacacionId))]
        public virtual PoliticaVacacion? PoliticaVacacion { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal AniosAntiguedad { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal DiasVacaciones { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PrimaVacacional { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal DiasAguinaldo { get; set; }

        public int Orden { get; set; }
    }
}