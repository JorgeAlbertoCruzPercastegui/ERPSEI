using ERPSEI.Data.Entities.SAT.Catalogos;

namespace ERPSEI.Data.Entities.Cuentas
{
	public class CuentaContableProductoServicio
	{
		public int Id { get; set; }

		public int? CuentaContableId { get; set; }
		public CuentaContable? CuentaContable { get; set; }

		public int? ProductoServicioId;
		public ProductoServicio? ProductoServicio { get; set; }
	}
}
