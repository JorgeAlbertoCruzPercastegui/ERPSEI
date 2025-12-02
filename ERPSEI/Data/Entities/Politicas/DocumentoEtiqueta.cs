namespace ERPSEI.Data.Entities.Politicas
{
	public class DocumentoEtiqueta
	{
		public int Id { get; set; }

		public int DocumentoId { get; set; }
		public Documento? Documento { get; set; }

		public string Etiqueta { get; set; } = string.Empty;
	}
}
