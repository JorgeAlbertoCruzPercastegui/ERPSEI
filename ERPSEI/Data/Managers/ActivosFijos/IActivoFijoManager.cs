using ERPSEI.Data.Entities.ActivosFijos;
using System.Threading.Tasks;
using ERPSEI.Requests;


namespace ERPSEI.Data.Managers.ActivosFijos
{
    public interface IActivoFijoManager : IRCatalogoManager<ActivoFijo>
    {
        Task<List<ActivoFijo>> GetAllAsync(ERPSEI.Areas.ERP.Pages.ActivosFijosModel.InputFiltroModel? filtro = null);

        Task<int> CreateFromExcelAsync(ActivoFijo activoFijo);
        Task UpdateFromExcelAsync(ActivoFijo activoFijo);

    }
}
