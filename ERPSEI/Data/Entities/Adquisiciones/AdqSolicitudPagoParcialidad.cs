
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_SolicitudesPagoParcialidades")]
    public class AdqSolicitudPagoParcialidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitudPagoId { get; set; }

        [Required]
        public int NumeroParcialidad { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal Porcentaje { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Importe { get; set; }

        [Required]
        [StringLength(30)]
        public string Estatus { get; set; } = "Pendiente";

        public DateTime? FechaPago { get; set; }

        [StringLength(450)]
        public string? UsuarioPagoId { get; set; }

        [StringLength(250)]
        public string? ReferenciaPago { get; set; }

        public DateTime FechaCreacion { get; set; }
            = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public bool Eliminado { get; set; } = false;

        [ForeignKey(nameof(SolicitudPagoId))]
        public AdqSolicitudPago SolicitudPago { get; set; }
            = null!;

        public string? ComprobanteNombreAlmacenado { get; set; }
        public string? ComprobanteNombreOriginal { get; set; }
        public string? ComprobanteHashSha256 { get; set; }
    }
}