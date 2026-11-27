using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Services.Adquisiciones;

namespace ERPSEI.Areas.ERP.Pages.Adquisiciones
{
    public partial class IndexModel
    {
        private const int TamanoPaginaCentralHu6 = 25;
        public bool MostrarBandejasCentralesHu6 =>
            _alcanceAdquisiciones.UsuarioValido &&
            _alcanceAdquisiciones.EsAdministradorAdquisiciones && AbacPuedeVisualizar;

        [BindProperty(SupportsGet = true)]
        public string? BuscarCentralHu6 { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PaginaCentralHu6 { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PaginaPagosHu6 { get; set; } = 1;
        public int TotalCentralHu6 { get; private set; }
        public int TotalPagosHu6 { get; private set; }
        public int PaginasCentralHu6 => Math.Max(1, (int)Math.Ceiling(TotalCentralHu6 / (double)TamanoPaginaCentralHu6));
        public int PaginasPagosHu6 => Math.Max(1, (int)Math.Ceiling(TotalPagosHu6 / (double)TamanoPaginaCentralHu6));
        public List<MonitoreoCentralHu6Dto> MonitoreoCentralHu6 { get; private set; } = new();
        public List<PagoCentralHu6Dto> PagosCentralesHu6 { get; private set; } = new();

        public class MonitoreoCentralHu6Dto
        {
            public int Id { get; set; }
            public string Folio { get; set; } = "";
            public string Titulo { get; set; } = "";
            public string Solicitante { get; set; } = "";
            public string Area { get; set; } = "";
            public string Estatus { get; set; } = "";
            public int EstatusId { get; set; }
            public DateTime FechaCreacion { get; set; }
        }
        public sealed class PagoCentralHu6Dto : MonitoreoCentralHu6Dto
        {
            public string GeneradoPor { get; set; } = "";
            public string Proveedor { get; set; } = "";
            public string Moneda { get; set; } = "";
            public decimal Total { get; set; }
            public DateTime? FechaGeneracion { get; set; }
            public bool Verificado { get; set; }
            public int PagoId { get; set; }
            public DateTime? UltimaDescarga { get; set; }
            public string? DescargadoPor { get; set; }
        }

        private async Task CargarBandejasCentralesHu6Async()
        {
            if (!MostrarBandejasCentralesHu6) return;
            var ct = HttpContext.RequestAborted;
            var texto = (BuscarCentralHu6 ?? "").Trim();
            if (texto.Length > 150) texto = texto.Substring(0, 150);
            BuscarCentralHu6 = texto;

            var solicitudes = from s in _context.AdqSolicitudes.AsNoTracking()
                              join e in _context.Empleados.AsNoTracking() on s.EmpleadoSolicitanteId equals e.Id into ej
                              from e in ej.DefaultIfEmpty()
                              join a in _context.Areas.AsNoTracking() on s.AreaId equals a.Id into aj
                              from a in aj.DefaultIfEmpty()
                              join estado in _context.AdqEstatus.AsNoTracking() on s.EstatusId equals estado.Id
                              where !s.Eliminado
                              select new MonitoreoCentralHu6Dto
                              {
                                  Id = s.Id,
                                  Folio = s.Folio,
                                  Titulo = s.Titulo,
                                  Solicitante = e != null ? e.NombreCompleto : s.UsuarioSolicitanteId,
                                  Area = a != null ? a.Nombre : "Sin área",
                                  Estatus = estado.Nombre,
                                  EstatusId = s.EstatusId,
                                  FechaCreacion = s.FechaCreacion
                              };
            if (!string.IsNullOrEmpty(texto))
                solicitudes = solicitudes.Where(s => s.Folio.Contains(texto) || s.Titulo.Contains(texto) || s.Solicitante.Contains(texto) || s.Area.Contains(texto) || s.Estatus.Contains(texto));
            TotalCentralHu6 = await solicitudes.CountAsync(ct);
            PaginaCentralHu6 = Math.Clamp(PaginaCentralHu6, 1, PaginasCentralHu6);
            MonitoreoCentralHu6 = await solicitudes.OrderByDescending(s => s.FechaCreacion).ThenByDescending(s => s.Id)
                .Skip((PaginaCentralHu6 - 1) * TamanoPaginaCentralHu6).Take(TamanoPaginaCentralHu6).ToListAsync(ct);

            var pagos = from s in solicitudes
                        join p in _context.AdqSolicitudesPago.AsNoTracking() on s.Id equals p.SolicitudId
                        where !p.Eliminado && p.PdfGenerado
                        select new PagoCentralHu6Dto
                        {
                            Id = s.Id,
                            Folio = s.Folio,
                            Titulo = s.Titulo,
                            Solicitante = s.Solicitante,
                            Area = s.Area,
                            Estatus = s.Estatus,
                            EstatusId = s.EstatusId,
                            FechaCreacion = s.FechaCreacion,
                            GeneradoPor = _context.Users.Where(u => u.Id == p.UsuarioGeneracionId)
                                .Select(u => u.Empleado != null ? u.Empleado.NombreCompleto : u.UserName).FirstOrDefault() ?? "Sin información",
                            Proveedor = p.NombreProveedor ?? "",
                            Moneda = p.Moneda ?? "",
                            Total = p.Total,
                            PagoId = p.Id,
                            FechaGeneracion = p.FechaGeneracion
                        };
            TotalPagosHu6 = await pagos.CountAsync(ct);
            PaginaPagosHu6 = Math.Clamp(PaginaPagosHu6, 1, PaginasPagosHu6);
            PagosCentralesHu6 = await pagos.OrderByDescending(p => p.FechaGeneracion).ThenByDescending(p => p.Id)
                .Skip((PaginaPagosHu6 - 1) * TamanoPaginaCentralHu6).Take(TamanoPaginaCentralHu6).ToListAsync(ct);
            foreach (var pago in PagosCentralesHu6)
            {
                pago.Verificado = await AdquisicionesIntegridadPago.ObtenerPdfValidadoAsync(_context, pago.Id, ct) != null;
                var clave = $"PagoId={pago.PagoId};";
                var descarga = await (from h in _context.AdqHistorial.AsNoTracking()
                                      join u in _context.Users.AsNoTracking() on h.UsuarioId equals u.Id
                                      where h.SolicitudId == pago.Id && h.TipoEvento == "PAGO_CENTRAL_DESCARGA" && h.Descripcion.StartsWith(clave)
                                      orderby h.FechaEvento descending, h.Id descending
                                      select new { h.FechaEvento, Nombre = u.Empleado != null ? u.Empleado.NombreCompleto : u.UserName }).FirstOrDefaultAsync(ct);
                pago.UltimaDescarga = descarga?.FechaEvento;
                pago.DescargadoPor = descarga?.Nombre;
            }
        }
    }
}
