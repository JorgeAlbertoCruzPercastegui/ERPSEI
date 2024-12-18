using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
	public class ComprobanteManager(ApplicationDbContext db) : IComprobanteManager
    {
		private readonly List<TipoComprobante> tiposComprobante = [..db.TiposComprobante];
		private readonly List<Moneda> monedas = [.. db.Monedas];
		private readonly List<MetodoPago> metodosPago = [.. db.MetodosPago];
		private readonly List<FormaPago> formasPago = [.. db.FormasPago];
		private readonly List<UsoCFDI> usosCFDI = [.. db.UsosCFDI];

		private async Task<int> GetNextId()
        {
            List<Comprobante> registros = await db.Comprobantes.ToListAsync();
            Comprobante? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

		public async Task<int> CreateAsync(Comprobante c)
		{
			c.Id = await GetNextId();
			db.Comprobantes.Add(c);
			await db.SaveChangesAsync();
			return c.Id;
		}
		public async Task UpdateAsync(Comprobante c)
		{
			Comprobante? n = await db.FindAsync<Comprobante>(c.Id);
			if (n != null)
			{
				n.Conciliado = c.Conciliado;
				n.Cancelado = c.Cancelado;
				n.Contabilizado = c.Contabilizado;
				n.Valido = c.Valido;
				await db.SaveChangesAsync();
			}
		}

        public async Task DeleteAsync(Comprobante c)
        {
            db.Comprobantes.Remove(c);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
			Comprobante? c = await db.FindAsync<Comprobante>(id);
			if (c != null)
            {
                db.Remove(c);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
					Comprobante? c = await db.FindAsync<Comprobante>(int.Parse(id));
                    if (c != null)
                    {
                        db.Remove(c);
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

        public async Task<List<Comprobante>> GetAllAsync()
        {
            return await db.Comprobantes
                .Include(c => c.Complemento).ThenInclude(e => e.TimbreFiscalDigital)
                .ToListAsync();
        }

		public async Task<List<Comprobante>> GetAllAsync(
			string? empresaRFC = null,
			string? anio = null,
			string? mes = null,
			int? estatusId = null,
			int? tipoId = null,
			int? estatusContableId = null,
			string? tipoComprobanteClave = null,
			string? formaPagoClave = null,
			string? metodoPagoClave = null,
			string? usoCFDIClave = null,
			string? emisorRFC = null,
			string? receptorRFC = null
		)
		{

			List<Comprobante> lc = await db.Comprobantes
				.Where(e => tipoComprobanteClave == null || e.TipoDeComprobante == tipoComprobanteClave)
				.Where(e => formaPagoClave == null || e.FormaPago == formaPagoClave)
				.Where(e => metodoPagoClave == null || e.MetodoPago == metodoPagoClave)
				.Where(e => usoCFDIClave == null || (e.Receptor != null && e.Receptor.UsoCFDI == usoCFDIClave))
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.Where(e => emisorRFC == null || (e.Emisor != null && e.Emisor.Rfc == emisorRFC))
				.Where(e => receptorRFC == null || (e.Receptor != null && e.Receptor.Rfc == receptorRFC))
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.ToListAsync();

			if (empresaRFC != null) { lc = lc.FindAll(c => (c.Emisor != null && c.Emisor.Rfc == empresaRFC) || (c.Receptor != null && c.Receptor.Rfc == empresaRFC)); }
			if(estatusId != null) { 
				if(estatusId == 1)
				{
					lc = lc.FindAll(c => c.Valido == true); 
				}
				else if (estatusId == 2)
				{
					lc = lc.FindAll(c => c.Cancelado == true);
				}
			}
			switch (tipoId)
			{
				case 1:
					lc = lc.FindAll(c => c.Emisor != null && c.Emisor.Rfc == empresaRFC);
					break;
				case 2:
					lc = lc.FindAll(c => c.Receptor != null && c.Receptor.Rfc == empresaRFC);
					break;
				default:
					break;
			}
			if (estatusContableId != null) { 
				if(estatusContableId == 2)
				{
					lc = lc.FindAll(c => (c.Contabilizado ?? false) == true); 
				}
				else if(estatusContableId == 1)
				{
					lc = lc.FindAll(c => (c.Contabilizado ?? false) == false);
				}
			}
            if (anio != null) { lc = lc.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy") == anio); }
			if (mes != null) { lc = lc.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("MM") == mes); }

			foreach (Comprobante c in lc)
			{
				c.TipoDeComprobante = tiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefault()?.Descripcion;
				c.Moneda = monedas.Where(m => m.Clave == c.Moneda).FirstOrDefault()?.Descripcion;
				c.MetodoPago = metodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefault()?.Descripcion;
				c.FormaPago = formasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefault()?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = usosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefault()?.Descripcion; }
			}

			return lc;
		}

		public async Task<Comprobante?> GetValidatableComprobanteByIdAsync(int id)
		{
			return await db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();
		}

        public async Task<Comprobante?> GetByIdAsync(int id)
        {
            return await db.Comprobantes
                .Where(e => e.Id == id)
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();
        }

		public async Task<Comprobante?> GetByIdWithDescripcionesAsync(int id)
		{
			Comprobante? c = await db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			if (c != null)
			{
				c.TipoDeComprobante = (await db.TiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefaultAsync())?.Descripcion;
				c.Moneda = (await db.Monedas.Where(m => m.Clave == c.Moneda).FirstOrDefaultAsync())?.Descripcion;
				c.MetodoPago = (await db.MetodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefaultAsync())?.Descripcion;
				c.FormaPago = (await db.FormasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefaultAsync())?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = (await db.UsosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefaultAsync())?.Descripcion; }
			}

			return c;
		}

        public async Task<List<Comprobante>> GetByDateRangeAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<Comprobante> ls = await db.Comprobantes
                .Include(c => c.Complemento)
                .ThenInclude(e => e.TimbreFiscalDigital)
                .ToListAsync();

            // Filtrar por rango de fechas
            ls = ls.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) >= fechaInicio);
            ls = ls.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) <= fechaFin);

            // Filtrar por clienteId si se proporciona
            /*if (clienteId.HasValue)
            {
                ls = ls.FindAll(c => c.Id == clienteId.Value);
            }*/

            return ls;
        }


        public async Task<Comprobante?> GetByNameAsync(string name)
		{
			Comprobante? c = await db.Comprobantes
				.Where(p => $"{(p.Serie ?? string.Empty).ToLower()}{(p.Folio ?? string.Empty).ToLower()}".Equals(name, StringComparison.CurrentCultureIgnoreCase))
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			if (c != null)
			{
				c.TipoDeComprobante = tiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefault()?.Descripcion;
				c.Moneda = monedas.Where(m => m.Clave == c.Moneda).FirstOrDefault()?.Descripcion;
				c.MetodoPago = metodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefault()?.Descripcion;
				c.FormaPago = formasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefault()?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = usosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefault()?.Descripcion; }
			}

			return c;
		}

		public async Task UpdateMultipleAsync(List<Comprobante> comprobantes)
		{
			//Inicia una transacción.
			await db.Database.BeginTransactionAsync();
			try
			{
				db.Comprobantes.UpdateRange(comprobantes);

				await db.SaveChangesAsync();

				await db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await db.Database.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Comprobante?> GetWithConceptosByIdAsync(int id)
		{
			Comprobante? c = await db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Conceptos)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			return c;
		}

		public async Task<Comprobante?> GetWithReceptorByIdAsync(int id)
		{
			Comprobante? c = await db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			return c;
		}


		public async Task<List<Comprobante>> GetByRFCAsync(string rfc)
        {
            return await db.Comprobantes
                //.Include(c => c.Receptor)
                .Include(c => c.Emisor)
                .Include(c => c.Complemento).ThenInclude(e => e.TimbreFiscalDigital)
                .Where(c => c.Emisor.Rfc == rfc)
                //.Where(c => c.Receptor.Rfc == rfc || c.Emisor.Rfc == rfc)
                .ToListAsync();
        }

		public async Task<List<Comprobante>> GetComprobantesGraficas(
			string? anio = null,
			string? mes = null
		)
		{
			List<Comprobante> lc = await db.Comprobantes.Where(c => c.TipoDeComprobante == "I").Include(c => c.Emisor).ToListAsync();

			if (anio != null) { lc = [.. from Comprobante c in lc where DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy") == anio select c]; }

			if (mes != null) { lc = [.. from Comprobante c in lc where DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("MM") == mes select c]; }

			return lc;

		}

	}
}
