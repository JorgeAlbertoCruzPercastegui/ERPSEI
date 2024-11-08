using ERPSEI.Data.Entities.Cuentas;

namespace ERPSEI.Data.Managers.Cuentas
{
	public interface ICuentaContableProductoServicioManager : IRWCatalogoManager<CuentaContableProductoServicio>
	{

		public Task DeleteByCuentaIdAsync(int id);

	}
}