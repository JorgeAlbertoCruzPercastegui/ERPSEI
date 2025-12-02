namespace ERPSEI.Data.Entities.Politicas
{
	public class AuditoriaDocumento
	{
		public int Id { get; set; }

		public int DocumentoId { get; set; }
		public Documento? Documento { get; set; }

		public string UsuarioId { get; set; } = string.Empty;
		public string Accion { get; set; } = string.Empty; // ver, descargar, editar
		public DateTime Fecha { get; set; } = DateTime.Now;
		public string? Ip { get; set; }
	}
}
