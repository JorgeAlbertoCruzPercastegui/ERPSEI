using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Email;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services
{
    public class EventosProgramadosBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public EventosProgramadosBackgroundService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcesarEventosProgramadosAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcesarEventosProgramadosAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var ahora = DateTime.Now;

            var eventos = await db.Set<EventoIntranet>()
                .Where(x =>
                    x.Activo &&
                    x.EsProgramado &&
                    !x.Publicado &&
                    !x.NotificacionEnviada &&
                    x.FechaPublicacionProgramada.HasValue &&
                    x.FechaPublicacionProgramada.Value <= ahora)
                .ToListAsync(stoppingToken);

            foreach (var evento in eventos)
            {
                evento.Publicado = true;
                evento.FechaModificacion = ahora;

                await CrearNotificacionEventoAsync(db, emailSender, evento, stoppingToken);

                evento.NotificacionEnviada = true;
                evento.FechaNotificacion = ahora;
            }

            await db.SaveChangesAsync(stoppingToken);
        }

        private async Task CrearNotificacionEventoAsync(
            ApplicationDbContext db,
            IEmailSender emailSender,
            EventoIntranet evento,
            CancellationToken stoppingToken)
        {
            var usuarios = await db.Users
                .Include(u => u.Empleado)
                .Where(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Empleado != null &&
                    u.Empleado.Deshabilitado == 0)
                .ToListAsync(stoppingToken);

            string baseUrl = _configuration["Intranet:BaseUrl"] ?? "https://localhost:7106";
            string urlInterna = $"/Catalogos/Eventos?openId={evento.Id}";
            string urlCorreo = $"{baseUrl}{urlInterna}";

            var notificacion = new NotificacionIntranet
            {
                Titulo = "Nuevo evento publicado",
                Descripcion = evento.Titulo,
                Tipo = "Evento",
                Modulo = "Eventos",
                Url = urlInterna,
                Icono = "bi bi-calendar-event-fill",
                FechaPublicacion = DateTime.Now,
                Activa = true
            };

            foreach (var usuario in usuarios)
            {
                notificacion.UsuariosNotificados.Add(new NotificacionIntranetUsuario
                {
                    UserId = usuario.Id,
                    Leida = false,
                    FechaCreacion = DateTime.Now
                });
            }

            db.NotificacionesIntranet.Add(notificacion);

            string cuerpo = $@"
                <div style='font-family:Arial,sans-serif;color:#1f1466;'>

                    <div style='background:#1f1466;padding:18px 22px;border-radius:14px 14px 0 0;color:#ffffff;'>
                        <h2 style='margin:0;font-size:22px;'>Nuevo evento publicado</h2>
                    </div>

                    <div style='border:1px solid #e5e7eb;border-top:0;padding:24px;border-radius:0 0 14px 14px;background:#ffffff;'>

                        <p style='font-size:15px;color:#374151;'>Hola,</p>

                        <p style='font-size:15px;color:#374151;line-height:1.6;'>
                            Se ha publicado un nuevo evento en la intranet corporativa de SEI.
                        </p>

                        <div style='background:#f8f9ff;border-left:4px solid #1f1466;padding:16px;border-radius:10px;margin:18px 0;'>

                            <div style='font-size:18px;font-weight:700;color:#1f1466;margin-bottom:8px;'>
                                {evento.Titulo}
                            </div>

                            <div style='font-size:14px;color:#4b5563;line-height:1.5;'>
                                {evento.Descripcion}
                            </div>

                            <div style='font-size:13px;color:#4b5563;margin-top:10px;'>
                                Fecha del evento: <strong>{evento.FechaEvento:dd/MM/yyyy}</strong>
                            </div>

                        </div>

                        <p style='margin-top:24px;'>
                            <a href='{urlCorreo}'
                               style='display:inline-block;background:#1f1466;color:#ffffff;padding:12px 18px;border-radius:10px;text-decoration:none;font-weight:600;'>
                                Ver evento
                            </a>
                        </p>

                        <hr style='margin:28px 0;border:none;border-top:1px solid #e5e7eb;' />

                        <p style='font-size:12px;color:#6b7280;'>
                            Este correo fue enviado automáticamente desde la Intranet SEI.
                        </p>

                    </div>
                </div>";

            await emailSender.SendEmailAsync(
                "jcruz@asesorcliente.com",
                "Nuevo evento publicado - Intranet SEI",
                cuerpo
            );
        }
    }
}