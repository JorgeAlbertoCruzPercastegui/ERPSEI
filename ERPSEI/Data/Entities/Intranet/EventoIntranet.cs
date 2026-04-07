using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class EventoIntranet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Descripcion { get; set; }

        [StringLength(100)]
        public string? TipoEvento { get; set; } // Corporativo, Fiesta, Integración, etc.

        public DateTime FechaEvento { get; set; }

        public TimeSpan? HoraEvento { get; set; }

        public DateTime? FechaPublicacionProgramada { get; set; }

        public bool Publicado { get; set; } = false;

        public bool Activo { get; set; } = true;

        public bool EsProgramado { get; set; } = false;

        public bool RequiereGeolocalizacion { get; set; } = false;

        [StringLength(200)]
        public string? Region { get; set; }

        [StringLength(500)]
        public string? UrlFormulario { get; set; }

        [StringLength(50)]
        public string? TextoBoton { get; set; } = "Consulta aquí";

        [StringLength(500)]
        public string? RutaPortada { get; set; }

        [StringLength(255)]
        public string? NombrePortada { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string? CreadoPorId { get; set; }

        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPorId { get; set; }
    }
}