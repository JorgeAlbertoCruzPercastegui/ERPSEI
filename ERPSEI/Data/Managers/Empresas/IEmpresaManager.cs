using ERPSEI.Data.Entities.Empresas;

namespace ERPSEI.Data.Managers.Empresas
{
    public interface IEmpresaManager : IRWCatalogoManager<Empresa>
    {

        public Task<List<Empresa>> GetAllAsync(
            int? origen = null,
            int? nivel = null,
            int? actividadEconomica = null,
            string? rfc = null
        );

        public Task DisableByIdAsync(int id );

        public Task<Empresa?> GetByIdWithAdicionalesAsync(int id);

        public Task<Empresa?> GetByRFCAsync(string rfc);

        public Task<List<EmpresaBuscada>> SearchEmpresas(string texto);

        public Task<List<Empresa>> GetAllWithPerfilAndNivel();

	}
}