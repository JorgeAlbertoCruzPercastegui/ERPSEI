using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Email;

namespace ERPSEI.Services.CorreosDominios
{
    public class CorreoDominioCaducidadService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CorreoDominioCaducidadService> _logger;

        public CorreoDominioCaducidadService(
            IServiceProvider serviceProvider,
            ILogger<CorreoDominioCaducidadService> logger)
        {
            _serviceProvider = serviceProvider;
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al revisar caducidades de Correos y Dominios.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task RevisarCaducidadesAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            DateTime hoy = DateTime.Today;
            DateTime fechaAviso = hoy.AddDays(7);

            var administradoresTi = await userManager.GetUsersInRoleAsync("Administrador TI");

            if (!administradoresTi.Any())
                return;

            var registrosPorVencer = await db.CorreosDominios
                .Where(x =>
                    !x.Deshabilitado &&
                    x.FechaCaducacion.HasValue &&
                    x.FechaCaducacion.Value.Date == fechaAviso &&
                    !x.Notificacion7DiasEnviada)
                .ToListAsync(stoppingToken);

            if (!registrosPorVencer.Any())
                return;

            foreach (var registro in registrosPorVencer)
            {
                string titulo = "Dominio próximo a caducar";

                string descripcion =
                    $"El dominio {registro.Dominio} de la empresa {registro.Empresa} caduca el {registro.FechaCaducacion:dd/MM/yyyy}.";

                string url = "/Catalogos/CorreosDominios/CorreoDominio";

                await CrearNotificacionCampanitaAsync(
                    db,
                    administradoresTi,
                    titulo,
                    descripcion,
                    url,
                    stoppingToken);

                await EnviarCorreoAdministradoresTiAsync(
                    emailSender,
                    administradoresTi,
                    titulo,
                    descripcion,
                    registro);

                registro.Notificacion7DiasEnviada = true;
            }

            await db.SaveChangesAsync(stoppingToken);
        }

        private static async Task CrearNotificacionCampanitaAsync(
        ApplicationDbContext db,
        IList<AppUser> usuarios,
        string titulo,
        string descripcion,
        string url,
        CancellationToken stoppingToken)
        {
            var notificacion = new NotificacionIntranet
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Tipo = "CorreosDominios",
                Modulo = "Correos y Dominios",
                Url = url,
                Icono = "bi bi-envelope-exclamation-fill",
                FechaPublicacion = DateTime.Now,
                Activa = true,
                UserIdCreador = null
            };

            db.NotificacionesIntranet.Add(notificacion);
            await db.SaveChangesAsync(stoppingToken);

            foreach (var usuario in usuarios)
            {
                db.NotificacionesIntranetUsuarios.Add(new NotificacionIntranetUsuario
                {
                    NotificacionIntranetId = notificacion.Id,
                    UserId = usuario.Id,
                    Leida = false,
                    FechaCreacion = DateTime.Now
                });
            }

            await db.SaveChangesAsync(stoppingToken);
        }

        private async Task EnviarCorreoAdministradoresTiAsync(
    IEmailSender emailSender,
    IList<AppUser> usuarios,
    string titulo,
    string descripcion,
    CorreoDominio registro)
        {
            foreach (var usuario in usuarios)
            {
                if (string.IsNullOrWhiteSpace(usuario.Email))
                    continue;

                string cuerpoHtml = $@"
            <div style='font-family: Arial, sans-serif; color:#333;'>
                <h2 style='color:#21166f;'>Aviso de caducidad</h2>

                <p>Hola,</p>

                <p>
                    Se informa que el siguiente registro del módulo
                    <strong>Correos y Dominios</strong> está próximo a caducar.
                </p>

                <table style='border-collapse: collapse; width:100%; max-width:650px;'>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; font-weight:bold;'>Empresa</td>
                        <td style='border:1px solid #ddd; padding:8px;'>{registro.Empresa}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; font-weight:bold;'>Dominio</td>
                        <td style='border:1px solid #ddd; padding:8px;'>{registro.Dominio}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; font-weight:bold;'>Proveedor</td>
                        <td style='border:1px solid #ddd; padding:8px;'>{registro.Proveedor}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; font-weight:bold;'>Fecha Caducación</td>
                        <td style='border:1px solid #ddd; padding:8px; color:#dc3545; font-weight:bold;'>
                            {registro.FechaCaducacion:dd/MM/yyyy}
                        </td>
                    </tr>
                </table>

                <p style='margin-top:18px;'>
                    Favor de revisar este registro en la intranet para dar seguimiento oportuno.
                </p>

                <p style='color:#777; font-size:12px;'>
                    Este es un aviso automático generado por la Intranet.
                </p>
            </div>";

                try
                {
                    await emailSender.SendEmailAsync(
                        usuario.Email,
                        titulo,
                        cuerpoHtml);

                    _logger.LogInformation(
                        "Correo de caducidad enviado a {Email} para dominio {Dominio}",
                        usuario.Email,
                        registro.Dominio);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al enviar correo de caducidad a {Email}",
                        usuario.Email);
                }
            }
        }
    }
}