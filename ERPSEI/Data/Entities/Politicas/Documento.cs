using ERPSEI.Data.Entities.Empleados;

namespace ERPSEI.Data.Entities.Politicas
{
	public class Documento
	{
		public int Id { get; set; }

		// FK hacia Area (carpeta Empleados)
		public int AreaId { get; set; }
		public Area? Area { get; set; }

		// FK hacia TipoDocumento
		public int TipoDocumentoId { get; set; }
		public TipoDocumento? TipoDocumento { get; set; }

		// Campos base
		public string Titulo { get; set; } = string.Empty;
		public string? Descripcion { get; set; }
		public string? PalabrasClave { get; set; }

		public string RutaArchivo { get; set; } = string.Empty;
		public string VersionActual { get; set; } = "1.0";
		public string Estatus { get; set; } = "Vigente";

		public string? ResponsableId { get; set; } // AspNetUsers
		public DateTime FechaPublicacion { get; set; }
		public DateTime FechaRegistro { get; set; } = DateTime.Now;
		public DateTime? ProximaRevision { get; set; }
		public bool Activo { get; set; } = true;

		// Relaciones
		public ICollection<DocumentoVersion>? Versiones { get; set; }
		public ICollection<DocumentoAdjunto>? Adjuntos { get; set; }
		public ICollection<DocumentoRelacion>? DocumentosRelacionados { get; set; }
		public ICollection<DocumentoRelacion>? RelacionadoDe { get; set; }
		public ICollection<AuditoriaDocumento>? Auditorias { get; set; }
		public ICollection<DocumentoEtiqueta>? Etiquetas { get; set; }
	}
}
