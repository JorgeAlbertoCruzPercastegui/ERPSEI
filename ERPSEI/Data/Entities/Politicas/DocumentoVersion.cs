namespace ERPSEI.Data.Entities.Politicas
{
	public class DocumentoVersion
	{
		public int Id { get; set; }

		public int DocumentoId { get; set; }
		public Documento? Documento { get; set; }

		public string Version { get; set; } = "1.0";
		public string RutaArchivo { get; set; } = string.Empty;
		public string? Comentarios { get; set; }
		public string? UsuarioId { get; set; }
		public DateTime Fecha { get; set; } = DateTime.Now;
	}
}
