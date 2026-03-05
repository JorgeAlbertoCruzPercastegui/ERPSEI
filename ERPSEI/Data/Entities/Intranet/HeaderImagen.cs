using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class HeaderImagen
    {
        public int Id { get; set; }

        // Ej: "Navidad", "Halloween", "Verano", "Principal"
        [Required, MaxLength(80)]
        public string Temporada { get; set; } = "Principal";

        [MaxLength(150)]
        public string? Titulo { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required, MaxLength(300)]
        public string NombreArchivo { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string RutaArchivo { get; set; } = string.Empty; // /uploads/header/xxx.png

        public DateTime? VigenciaInicio { get; set; }
        public DateTime? VigenciaFin { get; set; }

        public bool EsPermanente { get; set; } = false;
        public bool Activo { get; set; } = true;

        public int Orden { get; set; } = 1;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? UsuarioCreadorId { get; set; }
        public AppUser? UsuarioCreador { get; set; }
    }
}