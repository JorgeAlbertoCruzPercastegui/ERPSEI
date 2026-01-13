using ERPSEI.Data.Entities.Documentos;

namespace ERPSEI.Data.Managers.Documentos
{
    public interface ITipoDocumentoManager : IRCatalogoManager<TipoDocumento>
    {
        Task<List<TipoDocumento>> GetAllAsync(ERPSEI.Areas.Reportes.Pages.DocumentacionModel.TipoDocumentoFiltroModel? filtro = null);
    }
}
