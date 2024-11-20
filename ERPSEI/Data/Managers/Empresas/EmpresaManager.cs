using ERPSEI.Data.Entities.Empresas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Empresas
{
	public class EmpresaManager : IEmpresaManager
    {
        ApplicationDbContext db { get; set; }

        public EmpresaManager(ApplicationDbContext _db)
        {
            db = _db;
        }

        private async Task<int> getNextId()
        {
            List<Empresa> registros = await db.Empresas.ToListAsync();
			Empresa? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(Empresa e)
        {
            e.Id = await getNextId();
            db.Empresas.Add(e);
            await db.SaveChangesAsync();
            return e.Id;
        }
        public async Task UpdateAsync(Empresa e)
        {
			Empresa? dbE = db.Find<Empresa>(e.Id);
            if (dbE != null)
            {
                dbE.RazonSocial = e.RazonSocial;
				dbE.OrigenId = e.OrigenId;
				dbE.NivelId = e.NivelId;
				dbE.RFC = e.RFC;
				dbE.DomicilioFiscal = e.DomicilioFiscal;
				dbE.Administrador = e.Administrador;
				dbE.Accionista = e.Accionista;
				dbE.CorreoGeneral = e.CorreoGeneral;
				dbE.CorreoBancos = e.CorreoBancos;
				dbE.CorreoFiscal = e.CorreoFiscal;
				dbE.Telefono = e.Telefono;
				await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Empresa e)
        {
            db.Remove(e);
            await db.SaveChangesAsync();
        }

        public async Task DisableByIdAsync(int id)
        {
            Empresa? empresa = await GetByIdAsync(id);

            if (empresa != null)
            {
                //Deshabilita el empleado
                empresa.Deshabilitado = 1;
                
                db.Update(empresa);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
			Empresa? e = await GetByIdAsync(id);
            if (e != null)
            {
                db.Remove(e);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            //Inicia una transacción.
            await db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
					Empresa? e = await GetByIdAsync(int.Parse(id));
                    if (e != null)
                    {
                        db.Remove(e);
                        await db.SaveChangesAsync();
                    }
                }

                await db.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await db.Database.RollbackTransactionAsync();
                throw;

            }
        }

        public async Task<List<Empresa>> GetAllAsync(
            int? origenId = null,
            int? nivelId = null,
            int? actividadEconomicaId = null,
            string? rfc = null
        )
        {
            return await db.Empresas
                .Where(e => e.Deshabilitado == 0)
                .Where(e => origenId != null ? e.OrigenId == origenId : true)
                .Where(e => nivelId != null ? e.NivelId == nivelId : true)
                .Where(e => rfc != null ? e.RFC == rfc : true)
                .Include(e => e.Origen)
                .Include(e => e.Nivel)
                .Include(e => e.Perfil)
                .Include(e => e.ActividadesEconomicasEmpresa)
                .Where(e => actividadEconomicaId != null ? e.ActividadesEconomicasEmpresa.Any(a => a.ActividadEconomicaId == actividadEconomicaId) : true)
                .ToListAsync();
        }

        public async Task<List<Empresa>> GetAllAsync()
        {
			return await GetAllAsync(null, null, null);
		}

        public async Task<Empresa?> GetByIdWithAdicionalesAsync(int id)
        {
            return await db.Empresas
                .Where(e => e.Deshabilitado == 0)
                .Where(e => e.Id == id)
                .Include(e => e.BancosEmpresa)
                .Include(e => e.ActividadesEconomicasEmpresa)
                .ThenInclude(a => a.ActividadEconomica).FirstOrDefaultAsync();
        }

        public async Task<Empresa?> GetByIdAsync(int id)
        {
            return await db.Empresas.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Empresa?> GetByRFCAsync(string rfc)
        {
            return await db.Empresas.Where(e => e.RFC == rfc).FirstOrDefaultAsync();
        }

        public async Task<Empresa?> GetByNameAsync(string razonSocial)
        {
            return await db.Empresas.Where(e => e.RazonSocial.ToLower() == razonSocial.ToLower()).FirstOrDefaultAsync();
        }

		public async Task<List<EmpresaBuscada>> SearchEmpresas(string texto)
		{
			string sql = $"SELECT TOP (20) " +
                            $"Empresas.Id, " +
                            $"Empresas.RazonSocial, " +
                            $"Origenes.Nombre AS Origen, " +
                            $"Niveles.Nombre AS Nivel, " +
							$"Niveles.Ordinal AS Ordinal, " +
							$"Niveles.PuedeFacturar AS PuedeFacturar, " +
							$"Empresas.RFC, " +
                            $"Empresas.DomicilioFiscal, " +
                            $"Perfiles.Nombre AS Perfil, " +
                            $"Empresas.ObjetoSocial " +
                         $"FROM Empresas " +
						 $"LEFT JOIN Origenes ON Empresas.OrigenId = Origenes.Id " +
						 $"LEFT JOIN Niveles ON Empresas.NivelId = Niveles.Id " +
                         $"LEFT JOIN Perfiles ON Empresas.PerfilId = Perfiles.Id " +
						 $"WHERE RazonSocial LIKE '%{texto}%' OR RFC LIKE '%{texto}%'";
			List<EmpresaBuscada> emp = await db.Database.SqlQueryRaw<EmpresaBuscada>(sql).ToListAsync();

			return emp;
		}

        public async Task<List<Empresa>> GetAllWithPerfil() 
        {
			return await db.Empresas.Include(e => e.Perfil).ToListAsync();
		}

	}
}
