using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_PermisosComplianceEmpresasUsuario")]
    [Index(
        nameof(UsuarioId),
        nameof(EmpresaId),
        IsUnique = true
    )]
    public class EbPermisoComplianceEmpresaUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UsuarioId { get; set; } =
            string.Empty;

        public int EmpresaId { get; set; }

        public DateTime FechaCreacion { get; set; } =
            DateTime.Now;

        [MaxLength(450)]
        public string? UsuarioCreacionId { get; set; }
    }
}