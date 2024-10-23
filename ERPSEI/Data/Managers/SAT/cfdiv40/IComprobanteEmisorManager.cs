using ERPSEI.Data.Entities.SAT.cfdiv40;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
    public interface IComprobanteEmisorManager : IRCatalogoManager<ComprobanteEmisor>
    {
        public Task<List<ComprobanteEmisor>> SearchEmisor(string text);
	}
}