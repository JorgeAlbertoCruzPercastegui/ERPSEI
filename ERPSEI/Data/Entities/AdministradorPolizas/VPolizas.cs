using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class VPolizas
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public int GrupoId { get; set; }
		public GruposPolizas? Grupo { get; set; }
		public int TipoId { get; set; }
		public PolizasTipos? Tipo{ get; set; }
		public DateTime? FechaHora { get; set; }
		public string Concepto { get; set; } = string.Empty;
		public ICollection<PolizasDetalles>? PolizasDetalles { get; set; } = new List<PolizasDetalles>();
	}
}
