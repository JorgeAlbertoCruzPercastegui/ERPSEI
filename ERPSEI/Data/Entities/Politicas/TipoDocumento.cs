namespace ERPSEI.Data.Entities.Politicas
{
	public class TipoDocumento
	{
		public int Id { get; set; }
		public string Nombre { get; set; } = string.Empty;
		public string? Descripcion { get; set; }

		public ICollection<Documento>? Documentos { get; set; }
	}
}
