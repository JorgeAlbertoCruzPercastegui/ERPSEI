using ERPSEI.Data.Entities.ActivosFijos;

namespace ERPSEI.Data.Managers.ActivosFijos
{
    public interface IActivoFijoManager : IRCatalogoManager<ActivoFijo>
    {
        Task<List<ActivoFijo>> GetFilteredAsync(
            int? folio,
            string? responsable,
            int? categoriaId,
            int? tipoId,
            DateTime? fechaInicio,
            DateTime? fechaFin);
    }
}
