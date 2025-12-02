namespace ERPSEI.Data.Entities.Politicas
{
	public class DocumentoAdjunto
	{
		public int Id { get; set; }

		public int DocumentoId { get; set; }
		public Documento? Documento { get; set; }

		public string NombreArchivo { get; set; } = string.Empty;
		public string RutaArchivo { get; set; } = string.Empty;
		public string Tipo { get; set; } = string.Empty;
		public DateTime Fecha { get; set; } = DateTime.Now;
	}
}
