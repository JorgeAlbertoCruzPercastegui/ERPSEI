using ERPSEI.Data.Entities.Intranet;

namespace ERPSEI.Data.Managers.Intranet
{
    public interface IEventoIntranetManager
    {
        Task<List<EventoIntranet>> GetAllAsync(bool incluirInactivos = true);
        Task<EventoIntranet?> GetByIdAsync(int id);
        Task<EventoIntranet> AddAsync(EventoIntranet entity);
        Task<EventoIntranet> UpdateAsync(EventoIntranet entity);
        Task<bool> ToggleActivoAsync(int id, string? userId = null);
        Task<bool> PublicarAsync(int id, string? userId = null);
        Task<bool> DeleteAsync(int id, string? userId = null);
        Task<List<EventoIntranet>> GetPublicadosAsync(string? region = null);
    }
}