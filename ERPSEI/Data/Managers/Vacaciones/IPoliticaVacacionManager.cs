using ERPSEI.Data.Entities.Vacaciones;

namespace ERPSEI.Data.Managers.Vacaciones
{
    public interface IPoliticaVacacionManager
    {
        Task<List<PoliticaVacacion>> GetActivasAsync();
        Task<PoliticaVacacion?> GetPorTipoAsync(string tipoVacacion);
        Task<PoliticaVacacion?> GetByIdAsync(int id);
        Task<PoliticaVacacion> CreateAsync(PoliticaVacacion politica);
    }
}