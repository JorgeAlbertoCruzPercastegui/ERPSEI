using ERPSEI.Data.Entities.Vacaciones;

namespace ERPSEI.Data.Managers.Vacaciones
{
    public interface ISolicitudVacacionesManager : IRCatalogoManager<SolicitudVacaciones>
    {
        Task<List<SolicitudVacaciones>> GetAllAsync(ERPSEI.Areas.ERP.Pages.VacacionesModel.InputFiltroVacacionesModel? filtro = null);

        Task<int> CreateAsync(SolicitudVacaciones solicitudVacaciones);
    }
}
