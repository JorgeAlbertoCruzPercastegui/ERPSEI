using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_BitacoraDocumentos")]
    public class EbBitacoraDocumento
    {
        public long Id { get; set; }

        public int EmpresaId { get; set; }

        public int? DocumentoId { get; set; }

        public int? TipoDocumentoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;

        [StringLength(450)]
        public string? UsuarioId { get; set; }

        [StringLength(250)]
        public string? NombreUsuario { get; set; }

        [StringLength(250)]
        public string? NombreDocumento { get; set; }

        [StringLength(50)]
        public string? Banco { get; set; }

        public DateTime FechaEvento { get; set; }

        [StringLength(64)]
        public string? DireccionIp { get; set; }

        [StringLength(1000)]
        public string? Navegador { get; set; }

        public bool Exitoso { get; set; }

        [StringLength(1000)]
        public string? Detalle { get; set; }

        public int? VersionDocumento { get; set; }

        public EbEmpresa? Empresa { get; set; }

        public EbDocumento? Documento { get; set; }

        public EbTipoDocumento? TipoDocumento { get; set; }
    }
}