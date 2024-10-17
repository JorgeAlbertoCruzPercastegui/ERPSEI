namespace ERPSEI.Data.Entities.Cuentas
{
	public class CuentaContableTipo
	{
		public int Id { get; set; }
		public string? Clave { get; set; }
		public string? Descripcion { get; set; }
		public ICollection<CuentaContable>? CuentasContables { get; set; } = [];
	}
}
