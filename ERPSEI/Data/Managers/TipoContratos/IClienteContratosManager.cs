using ERPSEI.Data.Entities.TipoContratos;

namespace ERPSEI.Data.Managers.TipoContratos
{
    public interface IClienteContratosManager : IRCatalogoManager<ClienteContrato>
    {
        Task<List<ClienteContrato>> GetByEmpresaContratoIdAsync(int empresaContratoId);
    }
}
