using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.Adquisiciones
{
    public static class AdquisicionesNotificacionPagoHu6
    {
        public static async Task<List<string>> RegistrarAsync(ApplicationDbContext context, int solicitudId, string usuarioCreadorId)
        {
            if (await AdquisicionesIntegridadPago.ObtenerPdfValidadoAsync(context, solicitudId) == null) return new();
            var pago = await context.AdqSolicitudesPago.AsNoTracking().SingleOrDefaultAsync(p => p.SolicitudId == solicitudId && !p.Eliminado && p.PdfGenerado);
            var solicitud = await context.AdqSolicitudes.AsNoTracking().SingleOrDefaultAsync(s => s.Id == solicitudId && !s.Eliminado);
            if (pago == null || solicitud == null) return new();
            // Serializa la creación por documento, sin mantener la transacción durante el envío de correo.
            await using var tx = await context.Database.BeginTransactionAsync();
            var recurso = $"ADQ-HU6-PAGO-{pago.Id}";
            await context.Database.ExecuteSqlInterpolatedAsync($@"DECLARE @r int; EXEC @r = sys.sp_getapplock @Resource={recurso}, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000; IF @r < 0 THROW 50001, 'No se obtuvo el bloqueo de notificacion.', 1;");
            const string titulo = "Nueva solicitud de pago recibida";
            string url = $"/ERP/Adquisiciones?openId={solicitudId}&pagoHu6={pago.Id}#pagosCentralHu6";
            if (await context.NotificacionesIntranet.AnyAsync(n => n.Modulo == "Adquisiciones" && n.Titulo == titulo && n.Url == url))
            {
                await tx.CommitAsync();
                return new();
            }
            var destinatarios = await (from u in context.Users.AsNoTracking()
                                       join ur in context.UserRoles on u.Id equals ur.UserId
                                       join r in context.Roles on ur.RoleId equals r.Id
                                       where !u.IsBanned && r.NormalizedName == "ADMINISTRADOR ADQUISICIONES"
                                           && context.AdqPermisosUsuarios.Any(p => p.UsuarioId == u.Id && (p.PuedeTodo || (p.PuedeVisualizar && p.PuedeDescargar)))
                                       select u.Id).Distinct().ToListAsync();
            if (destinatarios.Count == 0) { await tx.CommitAsync(); return destinatarios; }
            var ahora = DateTime.Now;
            var descripcion = $"{solicitud.Folio} - {solicitud.Titulo}. Área: {pago.AreaSolicitante}. Importe: {pago.Total:N2} {pago.Moneda}. Cuatro firmas y PDF verificado.";
            var n = new NotificacionIntranet
            {
                Titulo = titulo,
                Descripcion = descripcion.Length > 500 ? descripcion[..500] : descripcion,
                Tipo = "Adquisiciones",
                Modulo = "Adquisiciones",
                Url = url,
                Icono = "bi bi-file-earmark-check",
                FechaPublicacion = ahora,
                Activa = true,
                UserIdCreador = usuarioCreadorId
            };
            foreach (var id in destinatarios) n.UsuariosNotificados.Add(new NotificacionIntranetUsuario { UserId = id, Leida = false, FechaCreacion = ahora });
            context.NotificacionesIntranet.Add(n);
            await context.SaveChangesAsync();
            await tx.CommitAsync();
            return destinatarios;
        }
    }
}
