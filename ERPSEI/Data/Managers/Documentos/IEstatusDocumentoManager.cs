using ERPSEI.Data.Entities.Documentos;

namespace ERPSEI.Data.Managers.Documentos
{
    public interface IEstatusDocumentoManager : IRCatalogoManager<EstatusDocumento>
    {
        Task<List<EstatusDocumento>> GetAllAsync(ERPSEI.Areas.Reportes.Pages.DocumentacionModel.EstatusDocumentoFiltroModel? filtro = null);
    }
}
