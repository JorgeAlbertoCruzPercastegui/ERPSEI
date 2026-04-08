using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class ComunicadoInterno
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaPublicacion { get; set; } = DateTime.Today;

        public TimeSpan? HoraPublicacion { get; set; }

        public bool Publicado { get; set; } = false;

        public bool NotificacionEnviada { get; set; } = false;
        public DateTime? FechaNotificacion { get; set; }

        public bool EsPermanente { get; set; } = false;

        [StringLength(500)]
        public string? RutaPortada { get; set; }

        [StringLength(255)]
        public string? NombrePortada { get; set; }

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? NombreArchivo { get; set; }

        [StringLength(20)]
        public string? ExtensionArchivo { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string? CreadoPorId { get; set; }

        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPorId { get; set; }
    }
}