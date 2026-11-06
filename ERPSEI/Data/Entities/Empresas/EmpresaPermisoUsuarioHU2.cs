using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Empresas
{
    public class EmpresaPermisoUsuarioHU2
    {
        [Key]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public AppUser Usuario { get; set; } = null!;
        public bool PuedeVisualizar { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeCargarDocumentos { get; set; }
        public bool PuedeDescargar { get; set; }
        public bool PuedeDesactivar { get; set; }
        public bool PuedeEditarBancos { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        [StringLength(450)]
        public string? ActualizadoPorUserId { get; set; }
    }
}
