using ERPSEI.Data.Entities.Polizas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPSEI.Data.Managers.AdministradorPolizas
{
	public interface IGruposPolizasManager : IRWCatalogoManager<GrupoPoliza>
	{
		Task<List<GrupoPoliza>> GetAllAsync(
			int? id = null,
			string? usuarioCreador = null,
			string? usuarioModificador = null,
			DateTime? fechaHoraCreacion = null,
			DateTime? fechaHoraModificacion = null,
			int? numeroImpresion = null,
			bool deshabilitado = false
		);
	}
}
