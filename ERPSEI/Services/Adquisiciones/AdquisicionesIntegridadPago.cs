using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ERPSEI.Services.Adquisiciones
{
    // HU6: valida evidencias existentes, sin alterar el historial presupuestal.
    public static class AdquisicionesIntegridadPago
    {
        public static async Task<byte[]?> ObtenerPdfValidadoAsync(
            ApplicationDbContext context, int solicitudId, CancellationToken ct = default)
        {
            var pagos = await context.AdqSolicitudesPago.AsNoTracking()
                .Where(p => p.SolicitudId == solicitudId && !p.Eliminado)
                .Take(2).ToListAsync(ct);
            if (pagos.Count != 1) return null;
            var pago = pagos[0];
            if (!pago.PdfGenerado || !pago.AprobacionPresupuestalId.HasValue || pago.Total <= 0) return null;
            if (!await context.AdqSolicitudes.AsNoTracking().AnyAsync(s => s.Id == solicitudId && !s.Eliminado && s.EstatusId >= 13 && s.EstatusId <= 17, ct)) return null;
            var aprobacion = await context.AdqAprobacionesPresupuestales.AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == pago.AprobacionPresupuestalId.Value && a.SolicitudId == solicitudId && a.CotizacionId == pago.CotizacionId && !a.Eliminado && a.Estatus == "Aprobada", ct);
            if (aprobacion == null) return null;
            var etapas = await context.AdqAprobacionesPresupuestalesDetalle.AsNoTracking()
                .Where(d => d.AprobacionPresupuestalId == aprobacion.Id && !d.Eliminado)
                .OrderBy(d => d.Orden).ToListAsync(ct);
            if (!etapas.Select(d => d.Orden).SequenceEqual(new[] { 1, 2, 3, 4 }) || etapas.Any(d => d.Estatus != "Aprobada" || d.EsActual || !d.FechaDecision.HasValue || string.IsNullOrWhiteSpace(d.UsuarioAprobadorId))) return null;
            var ids = etapas.Select(d => d.Id).ToArray();
            var firmas = await context.AdqFirmasAprobacionesPresupuestales.AsNoTracking()
                .Where(f => ids.Contains(f.AprobacionPresupuestalDetalleId) && !f.Eliminado).ToListAsync(ct);
            if (firmas.Count != 4) return null;
            foreach (var etapa in etapas)
            {
                var evidencias = firmas.Where(f => f.AprobacionPresupuestalDetalleId == etapa.Id).ToList();
                if (evidencias.Count != 1) return null;
                var firma = evidencias[0];
                if (firma.Decision != "APROBAR" || firma.OrdenEtapa != etapa.Orden || firma.UsuarioFirmanteId != etapa.UsuarioAprobadorId || string.IsNullOrWhiteSpace(firma.HashContextoFirmado)) return null;
                var evidencia = await LeerValidadoAsync(firma.RutaFirmaSnapshot, firma.HashFirma,
                    Path.Combine("App_Data", "Adquisiciones", "FirmasAprobaciones", aprobacion.Id.ToString(), etapa.Id.ToString()), ct);
                if (evidencia == null) return null;
            }
            var plan = await context.AdqSolicitudesPagoParcialidades.AsNoTracking().Where(p => p.SolicitudPagoId == pago.Id && !p.Eliminado).ToListAsync(ct);
            if (plan.Count > 0 && (plan.Sum(p => p.Porcentaje) != 100m || plan.Sum(p => p.Importe) != pago.Total || plan.Any(p => p.Importe <= 0 || p.Porcentaje <= 0 || (p.Estatus != "Pendiente" && p.Estatus != "Pagada")))) return null;
            var pdf = await LeerValidadoAsync(pago.RutaArchivo, pago.HashArchivo,
                Path.Combine("App_Data", "Adquisiciones", "SolicitudesPago", solicitudId.ToString()), ct);
            return pdf != null && pdf.Length >= 5 && pdf[0] == 37 && pdf[1] == 80 && pdf[2] == 68 && pdf[3] == 70 && pdf[4] == 45 ? pdf : null;
        }

        private static async Task<byte[]?> LeerValidadoAsync(string? ruta, string? hash, string carpeta, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ruta) || string.IsNullOrWhiteSpace(hash) || hash.Length != 64) return null;
            try
            {
                string raiz = Directory.GetCurrentDirectory();
                string limite = Path.GetFullPath(Path.Combine(raiz, carpeta)).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                string archivo = Path.GetFullPath(Path.Combine(raiz, ruta.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));
                if (!archivo.StartsWith(limite, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) return null;
                // No aceptar enlaces simbólicos ni junctions dentro del árbol documental.
                for (string? actual = archivo; actual != null; actual = Path.GetDirectoryName(actual))
                {
                    if ((File.GetAttributes(actual) & FileAttributes.ReparsePoint) != 0) return null;
                    if (string.Equals(actual, raiz, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) break;
                }
                var info = new FileInfo(archivo);
                if (!info.Exists || info.Length <= 0 || info.Length > 50L * 1024 * 1024) return null;
                byte[] bytes = await File.ReadAllBytesAsync(archivo, ct);
                return string.Equals(Convert.ToHexString(SHA256.HashData(bytes)), hash, StringComparison.OrdinalIgnoreCase) ? bytes : null;
            }
            catch (IOException) { return null; }
            catch (UnauthorizedAccessException) { return null; }
            catch (ArgumentException) { return null; }
        }
    }
}
