using ERPSEI.Data.Entities.Empresas;

namespace ERPSEI.Data.Entities.Cuentas
{
	public class CuentaContable
	{
		public int Id { get; set; }
		public string? Cuenta { get; set; }
		public string? Nombre { get; set; }
		public string? RFC { get; set; }

		public int? EmpresaId { get; set; }
		public Empresa? Empresa { get; set; }

		public int? TipoId { get; set; }
		public CuentaContableTipo? Tipo { get; set; }

		public int? SubtipoId { get; set; }
		public CuentaContableSubtipo? Subtipo { get; set; }

		public ICollection<CuentaContableProductoServicio>? CuentasProductoServicio { get; set; } = [];
	}
}
