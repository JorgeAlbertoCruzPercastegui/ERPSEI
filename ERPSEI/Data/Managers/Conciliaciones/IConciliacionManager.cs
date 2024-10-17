using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Reportes;

namespace ERPSEI.Data.Managers.Conciliaciones
{
    public interface IConciliacionManager : IRWCatalogoManager<Conciliacion>
    {
        public Task<List<Conciliacion>> GetAllAsync(
            int? id = null,
            string? cliente = null,
            string? usuarioCreador = null,
            string? usuarioModificador = null,
            DateTime? fechaElaboracionInicio = null,
            DateTime? fechaElaboracionFin = null,
            bool deshabilitado = false
        );
    }
}
