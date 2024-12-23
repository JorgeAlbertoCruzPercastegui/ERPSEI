using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Cuentas;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class VPoliza
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public int? GrupoId { get; set; }
		public GrupoPoliza? Grupo { get; set; }
		public int TipoId { get; set; }
		public PolizaTipo? Tipo{ get; set; }
		public DateTime? FechaHora { get; set; }
		public string Concepto { get; set; } = string.Empty;
		public ICollection<PolizaDetalle>? PolizasDetalles { get; set; } = new List<PolizaDetalle>();
	}
}
