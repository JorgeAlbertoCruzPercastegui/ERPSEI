using ERPSEI.Data.Entities.SAT.cfdiv40;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
    public class ComprobanteManager : IComprobanteManager
    {
        private readonly ApplicationDbContext _db;

        public ComprobanteManager(ApplicationDbContext db)
        {
            _db = db;
        }

        private async Task<int> GetNextId()
        {
            List<Comprobante> registros = await _db.Comprobantes.ToListAsync();
            Comprobante? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

		public async Task<int> CreateAsync(Comprobante c)
		{
			c.Id = await GetNextId();
			_db.Comprobantes.Add(c);
			await _db.SaveChangesAsync();
			return c.Id;
		}
		public async Task UpdateAsync(Comprobante c)
		{
			Comprobante? n = await _db.FindAsync<Comprobante>(c.Id);
			if (n != null)
			{
				n.Conciliado = c.Conciliado;
				n.Cancelado = c.Cancelado;
				n.Contabilizado = c.Contabilizado;
				n.Valido = c.Valido;
				await _db.SaveChangesAsync();
			}
		}

        public async Task DeleteAsync(Comprobante c)
        {
            _db.Comprobantes.Remove(c);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
			Comprobante? c = await _db.FindAsync<Comprobante>(id);
			if (c != null)
            {
                _db.Remove(c);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
					Comprobante? c = await _db.FindAsync<Comprobante>(int.Parse(id));
                    if (c != null)
                    {
                        _db.Remove(c);
                        await _db.SaveChangesAsync();
                    }
                }

                await _db.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _db.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<Comprobante>> GetAllAsync()
        {
            return await _db.Comprobantes
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

			List<Comprobante> lc = await _db.Comprobantes
				.Where(e => tipoComprobanteClave == null || e.TipoDeComprobante == tipoComprobanteClave)
				.Where(e => formaPagoClave == null || e.FormaPago == formaPagoClave)
				.Where(e => metodoPagoClave == null || e.MetodoPago == metodoPagoClave)
				.Where(e => usoCFDIClave == null || (e.Receptor != null && e.Receptor.UsoCFDI == usoCFDIClave))
				.Where(e => emisorRFC == null || (e.Emisor != null && e.Emisor.Rfc == emisorRFC))
				.Where(e => receptorRFC == null || (e.Receptor != null && e.Receptor.Rfc == receptorRFC))
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
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
				c.TipoDeComprobante = (await _db.TiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefaultAsync())?.Descripcion;
				c.Moneda = (await _db.Monedas.Where(m => m.Clave == c.Moneda).FirstOrDefaultAsync())?.Descripcion;
				c.MetodoPago = (await _db.MetodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefaultAsync())?.Descripcion;
				c.FormaPago = (await _db.FormasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefaultAsync())?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = (await _db.UsosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefaultAsync())?.Descripcion; }
			}

			return lc;
		}

		public async Task<Comprobante?> GetValidatableComprobanteByIdAsync(int id)
		{
			return await _db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();
		}

        public async Task<Comprobante?> GetByIdAsync(int id)
        {
            return await _db.Comprobantes
                .Where(e => e.Id == id)
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();
        }

		public async Task<Comprobante?> GetByIdWithDescripcionesAsync(int id)
		{
			Comprobante? c = await _db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			if (c != null)
			{
				c.TipoDeComprobante = (await _db.TiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefaultAsync())?.Descripcion;
				c.Moneda = (await _db.Monedas.Where(m => m.Clave == c.Moneda).FirstOrDefaultAsync())?.Descripcion;
				c.MetodoPago = (await _db.MetodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefaultAsync())?.Descripcion;
				c.FormaPago = (await _db.FormasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefaultAsync())?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = (await _db.UsosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefaultAsync())?.Descripcion; }
			}

			return c;
		}

        public async Task<List<Comprobante>> GetByDateRangeAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<Comprobante> ls = await _db.Comprobantes
                .Include(c => c.Complemento)
                .ThenInclude(e => e.TimbreFiscalDigital)
                .ToListAsync();

            ls = ls.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) >= fechaInicio);
            ls = ls.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) <= fechaFin);

            return ls;
        }

		public async Task<Comprobante?> GetByNameAsync(string name)
		{
			Comprobante? c = await _db.Comprobantes
				.Where(p => $"{(p.Serie ?? string.Empty).ToLower()}{(p.Folio ?? string.Empty).ToLower()}".Equals(name, StringComparison.CurrentCultureIgnoreCase))
				.Include(e => e.Impuestos).ThenInclude(i => i.Traslados)
				.Include(e => e.Complemento).ThenInclude(c => c.TimbreFiscalDigital)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			if (c != null)
			{
				c.TipoDeComprobante = (await _db.TiposComprobante.Where(t => t.Clave == c.TipoDeComprobante).FirstOrDefaultAsync())?.Descripcion;
				c.Moneda = (await _db.Monedas.Where(m => m.Clave == c.Moneda).FirstOrDefaultAsync())?.Descripcion;
				c.MetodoPago = (await _db.MetodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefaultAsync())?.Descripcion;
				c.FormaPago = (await _db.FormasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefaultAsync())?.Descripcion;
				if (c.Receptor != null) { c.Receptor.UsoCFDI = (await _db.UsosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefaultAsync())?.Descripcion; }
			}

			return c;
		}

		public async Task UpdateMultipleAsync(List<Comprobante> comprobantes)
		{
			//Inicia una transacción.
			await _db.Database.BeginTransactionAsync();
			try
			{
				_db.Comprobantes.UpdateRange(comprobantes);

				await _db.SaveChangesAsync();

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await _db.Database.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Comprobante?> GetWithConceptosByIdAsync(int id)
		{
			Comprobante? c = await _db.Comprobantes
				.Where(e => e.Id == id)
				.Include(e => e.Conceptos)
				.Include(e => e.Emisor)
				.Include(e => e.Receptor)
				.FirstOrDefaultAsync();

			return c;
		}

        public async Task<List<Comprobante>> GetByRFCAsync(string rfc)
        {
            return await _db.Comprobantes
                .Include(c => c.Receptor)
                .Include(c => c.Emisor)
                .Include(c => c.Complemento).ThenInclude(e => e.TimbreFiscalDigital)
                .Where(c => c.Receptor.Rfc == rfc || c.Emisor.Rfc == rfc)
                .ToListAsync();
        }
    }
}
