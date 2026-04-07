using ERPSEI.Data.Entities.Intranet;

namespace ERPSEI.Data.Managers.Intranet
{
    public interface IComunicadoInternoManager
    {
        Task<List<ComunicadoInterno>> GetAllAsync(bool incluirInactivos = true);
        Task<ComunicadoInterno?> GetByIdAsync(int id);
        Task<ComunicadoInterno> AddAsync(ComunicadoInterno entity);
        Task<ComunicadoInterno> UpdateAsync(ComunicadoInterno entity);
        Task<bool> ToggleActivoAsync(int id, string? userId = null);
        Task<bool> PublicarAsync(int id, string? userId = null);
        Task<bool> DeleteAsync(int id, string? userId = null);
        Task<List<ComunicadoInterno>> GetPublicadosVisiblesAsync(int? mes = null);
    }
}