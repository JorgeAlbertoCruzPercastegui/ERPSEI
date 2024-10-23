using ERPSEI.Data.Entities.SAT.cfdiv40;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
    public interface IComprobanteReceptorManager : IRCatalogoManager<ComprobanteReceptor>
    {
		public Task<List<ComprobanteReceptor>> SearchReceptor(string text);
	}
}