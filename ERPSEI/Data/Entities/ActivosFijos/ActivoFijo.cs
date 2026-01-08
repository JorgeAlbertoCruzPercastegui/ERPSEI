using ERPSEI.Data.Entities.Empleados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ActivosFijos
{
    public class ActivoFijo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        // Estas claves foráneas siguen el patrón PropiedadId + Propiedad de navegación
        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        public int CategoriaId { get; set; }
        public CategoriaActivoFijo? Categoria { get; set; }

        public int TipoId { get; set; }
        public TipoActivoFijo? Tipo { get; set; }

        public int? OficinaId { get; set; }
        public Oficina? Oficina { get; set; }

        public string Folio { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;

        public DateTime? FechaCompra { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public string Comentarios { get; set; } = string.Empty;
        public DateTime? FechaRenovacion { get; set; }

        public string LinkFacturaCompra { get; set; } = string.Empty;

        public int? Cantidades { get; set; }

        public bool Deshabilitado { get; set; } = false;
        public ICollection<ArchivoActivoFijo>? Archivos { get; set; }
    }
}
