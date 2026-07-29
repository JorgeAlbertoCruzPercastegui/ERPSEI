using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_Accionistas")]
    public class EbAccionista
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [Required]
        [StringLength(250)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(13)]
        public string? Rfc { get; set; }

        [Column(TypeName = "decimal(7,4)")]
        public decimal PorcentajeParticipacion { get; set; }

        [StringLength(100)]
        public string? Nacionalidad { get; set; }

        public bool EsRepresentanteLegal { get; set; } = false;

        public bool Deshabilitado { get; set; } = false;

        public bool Eliminado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioCreacionId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? UsuarioActualizacionId { get; set; }

        public EbEmpresa? Empresa { get; set; }
    }
}