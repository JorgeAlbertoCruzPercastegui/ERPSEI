using ERPSEI.Data.Entities.Cuentas;
namespace ERPSEI.Data.Managers.Cuentas
{
	public interface ICuentaContableManager : IRWCatalogoManager<CuentaContable>
	{
		public Task<List<CuentaContable>> GetByIdEmpresaAsync(int id);

		public Task<List<CuentaContable>> SearchCuentas(string text, string receptorRFC, int tipoCuentaId, int subtipoCuentaId);
	}
}
