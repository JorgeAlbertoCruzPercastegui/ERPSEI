namespace ERPSEI.Data.Entities.Politicas
{
	public class DocumentoRelacion
	{
		public int Id { get; set; }

		public int DocumentoId { get; set; }
		public Documento? Documento { get; set; }

		public int RelacionadoId { get; set; }
		public Documento? Relacionado { get; set; }

		public string TipoRelacion { get; set; } = "Relacionado";
	}
}
