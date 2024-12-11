using ERPSEI.Data.Entities.Cuentas;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class PolizasDetalles
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public int PolizaId { get; set; }
		public VPolizas? Poliza { get; set; }
		public int CuentaId { get; set; }
		public CuentaContable? Cuenta { get; set; }
		public string Concepto { get; set; } = string.Empty;
		public decimal Debe { get; set; }
		public decimal Haber { get; set; }
	}
}
