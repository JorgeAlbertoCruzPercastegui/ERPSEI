using ERPSEI.Data.Entities.SAT.cfdiv40;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Empresas
{
	[NotMapped]
	public class EmpresaBuscada
    {
		public int Id { get; set; }

		public string? RazonSocial { get; set; } = string.Empty;

		public string? Origen { get; set; } = string.Empty;

		public string? Nivel { get; set; } = string.Empty;

		public int? Ordinal { get; set; }

		public bool? PuedeFacturar {  get; set; }

		public string? RFC { get; set; } = string.Empty;

		public string? DomicilioFiscal { get; set; } = string.Empty;

		public string? Perfil { get; set; } = string.Empty;

		public string? ObjetoSocial { get; set; } = string.Empty;
	}
}
