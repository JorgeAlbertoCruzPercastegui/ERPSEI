using ERPSEI.Data.Entities.SAT.cfdiv40;
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
            Comprobante? c = await GetByIdAsync(id);
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
                    Comprobante? c = await GetByIdAsync(int.Parse(id));
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
            string? periodo = null,
            int? estatusId = null,
            int? tipoId = null,
            int? formaPagoId = null,
            int? metodoPagoId = null,
            int? usoCFDIId = null,
            int? emisorId = null,
            int? receptorId = null
        )
        {
            List<Comprobante> lc = await _db.Comprobantes
                .Where(e => estatusId == null || 1 == estatusId)
                .Where(e => tipoId == null || 1 == tipoId)
                .Where(e => formaPagoId == null || 1 == formaPagoId)
                .Where(e => metodoPagoId == null || 1 == metodoPagoId)
                .Where(e => usoCFDIId == null || 1 == usoCFDIId)
                .Where(e => emisorId == null || (1 == emisorId || 1 == emisorId))
                .Where(e => receptorId == null || (1 == receptorId || 1 == receptorId))
                .Include(e => e.Emisor)
                .Include(e => e.Receptor)
                .Include(e => e.Complemento) 
                .ThenInclude(e => e.TimbreFiscalDigital)
                .ToListAsync();

            foreach (Comprobante c in lc)
            {
                c.Moneda = (await _db.Monedas.Where(m => m.Clave == c.Moneda).FirstOrDefaultAsync())?.Descripcion;
                c.MetodoPago = (await _db.MetodosPago.Where(m => m.Clave == c.MetodoPago).FirstOrDefaultAsync())?.Descripcion;
                c.FormaPago = (await _db.FormasPago.Where(f => f.Clave == c.FormaPago).FirstOrDefaultAsync())?.Descripcion;
                if (c.Receptor != null)
                {
                    c.Receptor.UsoCFDI = (await _db.UsosCFDI.Where(u => u.Clave == c.Receptor.UsoCFDI).FirstOrDefaultAsync())?.Descripcion;
                }
            }

            if (periodo != null)
            {
                lc = lc.FindAll(c => DateTime.ParseExact(c.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM") == periodo);
            }

            return lc;
        }

        public async Task<Comprobante?> GetByIdAsync(int id)
        {
            return await _db.Comprobantes
                .Include(c => c.Complemento) 
                .ThenInclude(e => e.TimbreFiscalDigital)
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Comprobante?> GetByNameAsync(string name)
        {
            return await _db.Comprobantes.Where(p => $"{(p.Serie ?? string.Empty).ToLower()}{(p.Folio ?? string.Empty).ToLower()}".Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
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
    }
}
