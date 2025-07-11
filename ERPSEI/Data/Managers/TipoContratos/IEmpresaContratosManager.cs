using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.TipoContratos;

namespace ERPSEI.Data.Managers.TipoContratos
{
    public interface IEmpresaContratosManager : IRCatalogoManager<EmpresaContrato>
    {
        Task<List<EmpresaContrato>> GetAllAsync(ERPSEI.Areas.Reportes.Pages.GeneradorContratoModel.InputFiltroModel? filtro = null);
    }
}
