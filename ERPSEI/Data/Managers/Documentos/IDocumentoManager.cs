using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Documentos;

namespace ERPSEI.Data.Managers.Documentos
{
    public interface IDocumentoManager : IRCatalogoManager<Documento>
    {
        Task<List<Documento>> GetAllAsync(ERPSEI.Areas.Reportes.Pages.DocumentacionModel.InputFiltroModel? filtro = null);
    }
}
