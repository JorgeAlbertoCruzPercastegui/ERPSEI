using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_AlcanceComplianceUsuarios")]
    public class EbAlcanceComplianceUsuario
    {
        [Key]
        [MaxLength(450)]
        public string UsuarioId { get; set; } = string.Empty;

        public bool RestringirEmpresas { get; set; } = false;

        public DateTime FechaCreacion { get; set; } =
            DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [MaxLength(450)]
        public string? UsuarioModificacionId { get; set; }
    }
}