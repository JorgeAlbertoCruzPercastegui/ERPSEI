using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Migrations;

namespace ERPSEI.Data.Managers.AdministradorPolizas
{
	public interface IPolizasManager : IRCatalogoManager<VPoliza>
	{
        Task<List<VPoliza>> GetByGrupoIdAsync(int grupoId);
    }
}
