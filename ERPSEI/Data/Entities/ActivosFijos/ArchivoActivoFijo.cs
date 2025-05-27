using ERPSEI.Data.Entities.Empleados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ActivosFijos {
    public class ArchivoActivoFijo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivoFijoId { get; set; }
        public ActivoFijo ActivoFijo { get; set; }

        [Required]
        [StringLength(300)]
        public string NombreArchivo { get; set; } = "";

        [StringLength(10)]
        public string Extension { get; set; } = "";

        [StringLength(50)]
        public string MimeType { get; set; } = "";

        public byte[]? Contenido { get; set; } // Opcional: si decides guardar en DB

        [StringLength(500)]
        public string? RutaArchivo { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }

}
