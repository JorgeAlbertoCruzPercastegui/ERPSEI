using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Services.Adquisiciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.ERP.Pages.Adquisiciones
{
    public partial class IndexModel
    {
        public bool PuedeAutoAutorizarHu6 { get; private set; }

        private async Task NotificarPagoCentralHu6SeguroAsync(int solicitudId, int pagoId, string usuarioId)
        {
            try
            {
                var destinatarios = await AdquisicionesNotificacionPagoHu6.RegistrarAsync(_context, solicitudId, usuarioId);
                foreach (var destino in destinatarios)
                {
                    try { await _adquisicionesEmailService.NotificarPagoCentralHu6Async(pagoId, destino); }
                    catch (Exception ex) { _logger.LogError(ex, "HU6: error enviando correo de pago {PagoId} a {UsuarioId}", pagoId, destino); }
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "HU6: error registrando recepción del pago {PagoId}", pagoId); }
        }

        private async Task RegistrarDescargaCentralHu6Async(AdqSolicitud solicitud, AdqSolicitudPago pago, string usuarioId)
        {
            var alcance = await _adquisicionesAlcanceService.ObtenerAlcanceAsync(User, HttpContext.RequestAborted);
            if (!alcance.UsuarioValido || !alcance.EsAdministradorAdquisiciones) return;
            _context.AdqHistorial.Add(new AdqHistorial
            {
                SolicitudId = solicitud.Id,
                UsuarioId = usuarioId,
                TipoEvento = "PAGO_CENTRAL_DESCARGA",
                Descripcion = $"PagoId={pago.Id}; PDF autorizado para entrega por control central. Hash={pago.HashArchivo}",
                EstatusAnteriorId = solicitud.EstatusId,
                EstatusNuevoId = solicitud.EstatusId,
                FechaEvento = DateTime.Now,
                DireccionIp = ObtenerDireccionIp()
            });
            await _context.SaveChangesAsync();
        }

        // Excepción explícita de autorización inicial; no altera las cuatro firmas.
        public async Task<IActionResult> OnPostAutoAutorizarHu6Async(int solicitudId, string? motivo)
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Challenge();
            motivo = motivo?.Trim();
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 1000)
            { TempData["MensajeError"] = "Indica una justificación de hasta 1000 caracteres."; return RedirectToPage(); }
            await using var tx = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var alcance = await _adquisicionesAlcanceService.ObtenerAlcanceAsync(User, HttpContext.RequestAborted);
            var solicitud = await _context.AdqSolicitudes.SingleOrDefaultAsync(s => s.Id == solicitudId && !s.Eliminado);
            if (solicitud == null) return NotFound();
            if (!alcance.UsuarioValido || !alcance.AreasAutorizadas.Contains(solicitud.AreaId) || solicitud.UsuarioSolicitanteId != usuario.Id ||
                !await TienePermisoAbacAsync(usuario, "Editar") || !await _context.AdqPermisosUsuarios.AnyAsync(p => p.UsuarioId == usuario.Id && p.PuedeAutoAutorizarSolicitud)) return Forbid();
            if (solicitud.EstatusId != 2) { TempData["MensajeError"] = "La solicitud no está pendiente del jefe directo."; return RedirectToPage(); }
            var pendientes = await _context.AdqAprobaciones.Where(a => a.SolicitudId == solicitudId && a.TipoAprobacion == "GerenteArea" && a.Estatus == "Pendiente").ToListAsync();
            if (pendientes.Count != 1) { TempData["MensajeError"] = "No existe una autorización inicial pendiente única."; return RedirectToPage(); }
            var aprobacion = pendientes[0];
            var jefeId = aprobacion.UsuarioAprobadorId;
            var ahora = DateTime.Now;
            // Conserva el jefe original en la evidencia, registra como decisor al solicitante real.
            aprobacion.UsuarioAprobadorId = usuario.Id;
            aprobacion.Estatus = "Aprobada";
            aprobacion.Comentario = "AUTOAUTORIZACION: " + motivo;
            aprobacion.FechaRespuesta = ahora;
            solicitud.EstatusId = 3;
            solicitud.FechaModificacion = ahora;
            _context.AdqHistorial.Add(new AdqHistorial
            {
                SolicitudId = solicitudId,
                UsuarioId = usuario.Id,
                TipoEvento = "AUTOAUTORIZACION",
                Descripcion = $"Autoautorización inicial. Jefe original: {jefeId}. Motivo: {motivo}",
                EstatusAnteriorId = 2,
                EstatusNuevoId = 3,
                FechaEvento = ahora,
                DireccionIp = ObtenerDireccionIp()
            });
            await _context.SaveChangesAsync();
            var destinos = await ObtenerDestinatariosHu6Async(solicitud.AreaId, false);
            if (!string.IsNullOrWhiteSpace(jefeId) && await _context.Users.AnyAsync(u => u.Id == jefeId && !u.IsBanned)) destinos.Add(jefeId);
            await CrearNotificacionAdquisicionesAsync(destinos, "Solicitud autoautorizada", $"La solicitud {solicitud.Folio} fue autoautorizada por su solicitante. Continúa con la revisión de su área.", $"/ERP/Adquisiciones?openId={solicitudId}", usuario.Id);
            await tx.CommitAsync();
            TempData["MensajeExito"] = "Autoautorización registrada. La solicitud continúa con la revisión de su área.";
            return RedirectToPage();
        }
    }
}
