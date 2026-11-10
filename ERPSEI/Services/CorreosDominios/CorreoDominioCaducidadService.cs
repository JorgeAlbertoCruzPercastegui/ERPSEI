using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Email;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ERPSEI.Services.CorreosDominios
{
    public class CorreoDominioCaducidadService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CorreoDominioCaducidadService> _logger;

        public CorreoDominioCaducidadService(
            IServiceScopeFactory scopeFactory,
            ILogger<CorreoDominioCaducidadService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RevisarCaducidadesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al revisar caducidades de Correos y Dominios.");
                }

                try
                {
                    // Revisar cada hora; el historial controla que solo haya un aviso diario.
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private async Task RevisarCaducidadesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var configuracion = await db.ConfiguracionesNotificacionesCorreosDominios
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == 1, cancellationToken);

            if (configuracion == null || !configuracion.Habilitado)
                return;

            var hoy = FechaLocalMexico();
            var limite = hoy.AddDays(configuracion.DiasAnticipacion);

            var registros = await db.CorreosDominios
                .AsNoTracking()
                .Where(x => !x.Deshabilitado && x.FechaCaducacion.HasValue && x.FechaCaducacion.Value < limite.AddDays(1))
                .ToListAsync(cancellationToken);

            if (!configuracion.NotificarVencidos)
                registros = registros.Where(x => x.FechaCaducacion!.Value.Date >= hoy).ToList();

            var destinatarios = await db.DestinatariosNotificacionesCorreosDominios
                .AsNoTracking()
                .Where(x => x.Activo)
                .ToListAsync(cancellationToken);

            // Los destinatarios asociados a usuarios toman su correo actual.
            var idsDestinatarios = destinatarios
                .Where(d => !string.IsNullOrWhiteSpace(d.UserId))
                .Select(d => d.UserId!)
                .Distinct()
                .ToList();

            var usuariosDestinatarios = await db.Users
                .AsNoTracking()
                .Where(x => idsDestinatarios.Contains(x.Id))
                .Select(x => new { x.Id, x.Email })
                .ToListAsync(cancellationToken);

            List<string> CorreosPorTipo(string tipo) => destinatarios
                .Where(d => string.Equals(d.TipoDestinatario, tipo, StringComparison.OrdinalIgnoreCase))
                .Select(d => !string.IsNullOrWhiteSpace(d.UserId)
                    ? usuariosDestinatarios.FirstOrDefault(u => u.Id == d.UserId)?.Email
                    : d.CorreoElectronico)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var correosSeguimiento = CorreosPorTipo("Seguimiento");
            var correosEscalamiento = CorreosPorTipo("Escalamiento");

            // Campanita solo para destinatarios de Seguimiento vinculados a INTRANET.
            var usuariosSeguimiento = destinatarios
                .Where(d => d.TipoDestinatario == "Seguimiento" && !string.IsNullOrWhiteSpace(d.UserId))
                .Select(d => d.UserId!)
                .Distinct()
                .ToList();
            var usuariosCampanita = await db.Users.AsNoTracking()
                .Where(u => usuariosSeguimiento.Contains(u.Id))
                .ToListAsync(cancellationToken);

            // Los vencidos se agrupan en un solo correo diario por destinatario.
            // Esto evita que un historial grande de dominios venza la bandeja de entrada.
            var vencidos = registros
                .Where(x => x.FechaCaducacion!.Value.Date < hoy)
                .OrderBy(x => x.FechaCaducacion)
                .ThenBy(x => x.Empresa)
                .ToList();

            if (configuracion.NotificarVencidos && configuracion.RecordatoriosDiarios && vencidos.Count > 0)
            {
                var destinatariosResumen = correosSeguimiento
                    .Concat(correosEscalamiento)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var correo in destinatariosResumen)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await EnviarResumenVencidosSiCorrespondeAsync(
                        db, emailSender, vencidos, correo, hoy, cancellationToken);
                }
            }

            foreach (var registro in registros.Where(x => x.FechaCaducacion!.Value.Date >= hoy))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fechaCaducacion = registro.FechaCaducacion!.Value.Date;
                var diasRestantes = (fechaCaducacion - hoy).Days;

                if (diasRestantes < 0 && !configuracion.NotificarVencidos)
                    continue;

                // Si se desactiva la recurrencia, solo se avisa el primer día de la ventana.
                // Los avisos críticos siguen sujetos al escalamiento configurado.
                bool primerDia = diasRestantes == configuracion.DiasAnticipacion;
                bool escalamiento = diasRestantes <= configuracion.DiasEscalamiento;
                bool notificarSeguimiento = configuracion.RecordatoriosDiarios || primerDia;
                bool notificarEscalamiento = escalamiento && (configuracion.RecordatoriosDiarios || diasRestantes == configuracion.DiasEscalamiento);

                var correos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (notificarSeguimiento)
                    foreach (var correo in correosSeguimiento) correos.Add(correo);
                if (notificarEscalamiento)
                    foreach (var correo in correosEscalamiento) correos.Add(correo);

                string tipoAviso = diasRestantes < 0 ? "Vencido" : diasRestantes == 0 ? "CaducaHoy" : escalamiento ? "Critico" : "Preventivo";

                foreach (var correo in correos)
                {
                    await EnviarAvisoSiCorrespondeAsync(db, emailSender, registro, correo,
                        fechaCaducacion, hoy, diasRestantes, tipoAviso, cancellationToken);
                }

                // Campanita para los responsables de Seguimiento vinculados a usuarios INTRANET.
                if (notificarSeguimiento)
                {
                    foreach (var usuario in usuariosCampanita)
                    {
                        await CrearCampanitaSiCorrespondeAsync(db, usuario, registro,
                            fechaCaducacion, hoy, diasRestantes, tipoAviso, cancellationToken);
                    }
                }
            }
        }

        // Un correo consolidado por destinatario y día para todos los dominios vencidos.
        // El historial conserva una fila POR DOMINIO: permite saber qué elementos
        // se incluyeron en el resumen y evita repetirlos en una misma fecha.
        private async Task EnviarResumenVencidosSiCorrespondeAsync(
            ApplicationDbContext db, IEmailSender emailSender,
            List<CorreoDominio> vencidos, string correo, DateTime hoy,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(correo) || vencidos.Count == 0)
                return;

            string clave = correo.Trim().ToLowerInvariant();
            var ids = vencidos.Select(x => x.Id).ToList();

            var historiales = await db.HistorialNotificacionesCorreosDominios
                .Where(x => ids.Contains(x.CorreoDominioId) &&
                    x.FechaAviso == hoy &&
                    x.DestinatarioClave == clave &&
                    x.Canal == "Correo")
                .ToListAsync(cancellationToken);

            var indice = historiales.ToDictionary(
                x => (x.CorreoDominioId, x.FechaCaducacion.Date));

            var pendientes = new List<(CorreoDominio Registro, HistorialNotificacionCorreoDominio Historial)>();

            foreach (var registro in vencidos)
            {
                var caducidad = registro.FechaCaducacion!.Value.Date;
                indice.TryGetValue((registro.Id, caducidad), out var historial);

                // EnProceso es deliberadamente conservador: si el proceso murió tras
                // entregar el mensaje no reenviamos sin una conciliación manual.
                if (historial != null &&
                    (historial.Estado == "Enviado" || historial.Estado == "EnProceso" || historial.Intentos >= 3))
                    continue;

                if (historial == null)
                {
                    historial = new HistorialNotificacionCorreoDominio
                    {
                        CorreoDominioId = registro.Id,
                        FechaCaducacion = caducidad,
                        FechaAviso = hoy,
                        DestinatarioClave = clave,
                        CorreoElectronico = correo.Trim(),
                        TipoAviso = "Vencido",
                        Canal = "Correo",
                        Estado = "Pendiente",
                        Intentos = 0
                    };
                    db.HistorialNotificacionesCorreosDominios.Add(historial);
                }

                pendientes.Add((registro, historial));
            }

            if (pendientes.Count == 0)
                return;

            // Guardar la reserva antes de intentar entregar el correo.
            foreach (var item in pendientes)
            {
                item.Historial.Intentos++;
                item.Historial.Estado = "EnProceso";
                item.Historial.Error = null;
            }
            await db.SaveChangesAsync(cancellationToken);

            try
            {
                string filas = string.Join("", pendientes.Select(item =>
                {
                    var r = item.Registro;
                    int atraso = (hoy - r.FechaCaducacion!.Value.Date).Days;
                    return $@"<tr>
                      <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(r.Empresa ?? "Sin empresa")}</td>
                      <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(r.Dominio ?? "Sin dominio")}</td>
                      <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(r.Proveedor ?? "Sin proveedor")}</td>
                      <td style='border:1px solid #ddd;padding:8px'>{r.FechaCaducacion:dd/MM/yyyy}</td>
                      <td style='border:1px solid #ddd;padding:8px;color:#b42318'>{atraso}</td>
                    </tr>";
                }));

                var asunto = $"INTRANET | {pendientes.Count} dominio(s) vencido(s) - {hoy:dd/MM/yyyy}";
                var cuerpo = $@"<div style='font-family:Arial,sans-serif;color:#303442'>
                    <h2 style='color:#21166f'>Resumen diario de dominios vencidos</h2>
                    <p>Se identificaron <strong>{pendientes.Count}</strong> registros vencidos que requieren revisión.</p>
                    <table style='border-collapse:collapse;width:100%;font-size:13px'>
                      <thead><tr style='background:#21166f;color:white'>
                        <th style='padding:9px'>Empresa</th><th style='padding:9px'>Dominio</th>
                        <th style='padding:9px'>Proveedor</th><th style='padding:9px'>Caducidad</th>
                        <th style='padding:9px'>Días de atraso</th>
                      </tr></thead><tbody>{filas}</tbody>
                    </table>
                    <p>Por favor, confirma las renovaciones y actualiza su fecha de caducación en
                    el módulo <strong>Correos y Dominios</strong> de INTRANET.</p>
                    <p style='font-size:12px;color:#666'>Mensaje automático de INTRANET. Fecha de corte: {hoy:dd/MM/yyyy}.</p>
                    </div>";

                await emailSender.SendEmailAsync(correo.Trim(), asunto, cuerpo);

                foreach (var item in pendientes)
                {
                    item.Historial.Estado = "Enviado";
                    item.Historial.FechaEnvio = DateTime.UtcNow;
                    item.Historial.Error = null;
                }
                _logger.LogInformation("Resumen de {Cantidad} dominios vencidos enviado a {Correo}",
                    pendientes.Count, correo);
            }
            catch (Exception ex)
            {
                string error = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;
                foreach (var item in pendientes)
                {
                    item.Historial.Estado = "Error";
                    item.Historial.Error = error;
                }
                _logger.LogError(ex, "Error enviando resumen de dominios vencidos a {Correo}", correo);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task EnviarAvisoSiCorrespondeAsync(
            ApplicationDbContext db, IEmailSender emailSender, CorreoDominio registro,
            string correo, DateTime fechaCaducacion, DateTime hoy,
            int diasRestantes, string tipoAviso, CancellationToken cancellationToken)
        {
            var clave = correo.ToLowerInvariant();
            var historial = await db.HistorialNotificacionesCorreosDominios
                .FirstOrDefaultAsync(x => x.CorreoDominioId == registro.Id &&
                    x.FechaCaducacion == fechaCaducacion && x.FechaAviso == hoy &&
                    x.DestinatarioClave == clave && x.Canal == "Correo", cancellationToken);

            if (historial?.Estado == "Enviado" || historial?.Estado == "EnProceso" || (historial?.Intentos ?? 0) >= 3)
                return;

            if (historial == null)
            {
                historial = new HistorialNotificacionCorreoDominio
                {
                    CorreoDominioId = registro.Id,
                    FechaCaducacion = fechaCaducacion,
                    FechaAviso = hoy,
                    DestinatarioClave = clave,
                    CorreoElectronico = correo,
                    TipoAviso = tipoAviso,
                    Canal = "Correo",
                    Estado = "Pendiente",
                    Intentos = 0
                };
                db.HistorialNotificacionesCorreosDominios.Add(historial);
                await db.SaveChangesAsync(cancellationToken);
            }

            historial.Intentos++;
            historial.Estado = "EnProceso";
            historial.Error = null;
            await db.SaveChangesAsync(cancellationToken);

            try
            {
                var nombreDominio = WebUtility.HtmlEncode(registro.Dominio ?? "Sin dominio");
                var empresa = WebUtility.HtmlEncode(registro.Empresa ?? "Sin empresa");
                var proveedor = WebUtility.HtmlEncode(registro.Proveedor ?? "Sin proveedor");
                var mensaje = diasRestantes < 0
                    ? $"Venció hace {-diasRestantes} día(s)"
                    : diasRestantes == 0 ? "Caduca hoy" : $"Faltan {diasRestantes} día(s)";

                var asunto = $"INTRANET - {(diasRestantes <= 0 ? "Dominio vencido o por vencer hoy" : "Recordatorio de caducidad de dominio")}";
                var cuerpo = $@"<div style='font-family:Arial,sans-serif;color:#303442;max-width:650px'>
                    <h2 style='color:#21166f'>Aviso de caducidad - INTRANET</h2>
                    <p>El siguiente registro requiere seguimiento:</p>
                    <table style='border-collapse:collapse;width:100%'>
                    <tr><td style='padding:9px;border:1px solid #ddd'><b>Empresa</b></td><td style='padding:9px;border:1px solid #ddd'>{empresa}</td></tr>
                    <tr><td style='padding:9px;border:1px solid #ddd'><b>Dominio</b></td><td style='padding:9px;border:1px solid #ddd'>{nombreDominio}</td></tr>
                    <tr><td style='padding:9px;border:1px solid #ddd'><b>Proveedor</b></td><td style='padding:9px;border:1px solid #ddd'>{proveedor}</td></tr>
                    <tr><td style='padding:9px;border:1px solid #ddd'><b>Fecha de caducación</b></td><td style='padding:9px;border:1px solid #ddd'>{fechaCaducacion:dd/MM/yyyy}</td></tr>
                    <tr><td style='padding:9px;border:1px solid #ddd'><b>Estado</b></td><td style='padding:9px;border:1px solid #ddd;color:#b42318'><b>{mensaje}</b></td></tr>
                    </table>
                    <p>Favor de revisar y actualizar la fecha de caducación en el módulo Correos y Dominios si ya se realizó la renovación.</p>
                    <p style='font-size:12px;color:#666'>Aviso automático de INTRANET.</p></div>";

                await emailSender.SendEmailAsync(correo, asunto, cuerpo);
                historial.Estado = "Enviado";
                historial.FechaEnvio = DateTime.UtcNow;
                historial.Error = null;
                _logger.LogInformation("Caducidad enviada a {Email}. Registro {Id}; días {Dias}.", correo, registro.Id, diasRestantes);
            }
            catch (Exception ex)
            {
                historial.Estado = "Error";
                historial.Error = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message;
                _logger.LogError(ex, "Falló correo a {Email} para registro {Id}.", correo, registro.Id);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private static async Task CrearCampanitaSiCorrespondeAsync(
            ApplicationDbContext db, AppUser usuario, CorreoDominio registro,
            DateTime fechaCaducacion, DateTime hoy, int diasRestantes,
            string tipoAviso, CancellationToken cancellationToken)
        {
            string clave = "usuario:" + usuario.Id;
            if (clave.Length > 256)
                return;

            bool existe = await db.HistorialNotificacionesCorreosDominios
                .AnyAsync(x => x.CorreoDominioId == registro.Id &&
                    x.FechaCaducacion == fechaCaducacion && x.FechaAviso == hoy &&
                    x.DestinatarioClave == clave && x.Canal == "Campanita", cancellationToken);
            if (existe) return;

            string estado = diasRestantes < 0 ? "ya venció" : diasRestantes == 0 ? "vence hoy" : $"vence en {diasRestantes} día(s)";
            var notificacion = new NotificacionIntranet
            {
                Titulo = diasRestantes <= 0 ? "Dominio vencido" : "Recordatorio: dominio próximo a caducar",
                Descripcion = $"El dominio {registro.Dominio} de {registro.Empresa} {estado} ({fechaCaducacion:dd/MM/yyyy}).",
                Tipo = "CorreosDominios",
                Modulo = "Correos y Dominios",
                Url = "/Catalogos/CorreosDominios/CorreoDominio",
                Icono = "bi bi-envelope-exclamation-fill",
                FechaPublicacion = DateTime.Now,
                Activa = true,
                UserIdCreador = null
            };
            notificacion.UsuariosNotificados.Add(new NotificacionIntranetUsuario
            {
                UserId = usuario.Id,
                Leida = false,
                FechaCreacion = DateTime.Now
            });
            db.NotificacionesIntranet.Add(notificacion);
            db.HistorialNotificacionesCorreosDominios.Add(new HistorialNotificacionCorreoDominio
            {
                CorreoDominioId = registro.Id,
                FechaCaducacion = fechaCaducacion,
                FechaAviso = hoy,
                DestinatarioClave = clave,
                CorreoElectronico = usuario.Email,
                TipoAviso = tipoAviso,
                Canal = "Campanita",
                Estado = "Enviado",
                Intentos = 1,
                FechaEnvio = DateTime.UtcNow
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        private static DateTime FechaLocalMexico()
        {
            TimeZoneInfo zona;
            try { zona = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City"); }
            catch (TimeZoneNotFoundException)
            {
                zona = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date;
        }
    }
}
