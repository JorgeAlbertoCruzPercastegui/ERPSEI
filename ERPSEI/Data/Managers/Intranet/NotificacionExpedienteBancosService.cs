using System.Data;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace ERPSEI.Services.Empresas
{
    /// <summary>
    /// HU2 CA-05: notifica a Bancos cuando el expediente pasa de incompleto a completo.
    /// La campanita se confirma en base de datos; después se intentan enviar correos
    /// por Graph, sin hacer fallar el guardado de Empresa si el correo falla.
    /// </summary>
    public sealed class NotificacionExpedienteBancosService : INotificacionExpedienteBancosService
    {
        private readonly ApplicationDbContext _db;
        private readonly AppUserManager _users;
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphClient;
        private readonly ILogger<NotificacionExpedienteBancosService> _logger;

        public NotificacionExpedienteBancosService(
            ApplicationDbContext db,
            AppUserManager users,
            IConfiguration configuration,
            GraphServiceClient graphClient,
            ILogger<NotificacionExpedienteBancosService> logger)
        {
            _db = db;
            _users = users;
            _configuration = configuration;
            _graphClient = graphClient;
            _logger = logger;
        }

        public async Task EvaluarDespuesDeGuardarAsync(int empresaId, CancellationToken ct = default)
        {
            if (empresaId <= 0 || !_configuration.GetValue<bool>("HU2NotificacionesBancos:Habilitado"))
                return;

            // El correo se envía solamente si esta evaluación creó el aviso de campanita.
            string[] correosDestinatarios = Array.Empty<string>();
            string? razonSocialParaCorreo = null;
            string? rutaExpedienteParaCorreo = null;
            int notificacionId = 0;

            await using (var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct))
            {
                var empresa = await _db.Empresas.AsNoTracking()
                    .Where(e => e.Id == empresaId)
                    .Select(e => new { e.Id, e.RazonSocial, e.Deshabilitado })
                    .FirstOrDefaultAsync(ct);

                if (empresa == null || empresa.Deshabilitado != 0)
                    return;

                var tipos = await _db.Set<TipoArchivoEmpresa>().AsNoTracking()
                    .Where(t => t.EsObligatorio)
                    .Select(t => t.Id)
                    .OrderBy(x => x)
                    .ToListAsync(ct);

                if (tipos.Count == 0)
                    return;

                var disponibles = await _db.ArchivosEmpresa.AsNoTracking()
                    .Where(a => a.EmpresaId == empresaId && a.TipoArchivoId.HasValue && a.Archivo.Length > 0)
                    .Select(a => a.TipoArchivoId!.Value)
                    .Distinct()
                    .ToListAsync(ct);

                var cargados = disponibles.ToHashSet();
                bool completo = tipos.All(cargados.Contains);
                string firma = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(string.Join(",", tipos))));

                var estado = await _db.Set<EmpresaEstadoDocumentalHU2>()
                    .FindAsync(new object[] { empresaId }, ct);

                bool primeraEvaluacion = estado == null;
                if (estado == null)
                {
                    estado = new EmpresaEstadoDocumentalHU2
                    {
                        EmpresaId = empresaId,
                        FirmaRegla = firma,
                        Completo = completo,
                        FechaEvaluacion = DateTime.UtcNow
                    };
                    _db.Set<EmpresaEstadoDocumentalHU2>().Add(estado);
                }

                bool cambioRegla = !primeraEvaluacion && estado.FirmaRegla != firma;
                bool debeAvisar = !primeraEvaluacion && !cambioRegla && !estado.Completo && completo;

                if (debeAvisar)
                {
                    var roles = _configuration.GetSection("HU2NotificacionesBancos:RolesDestinatarios")
                        .GetChildren()
                        .Select(x => x.Value)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    if (roles.Length == 0)
                    {
                        _logger.LogWarning("HU2: sin roles destinatarios; expediente {EmpresaId}", empresaId);
                        return;
                    }

                    var destinatariosPorId = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                    foreach (string rol in roles)
                    {
                        var usuarios = await _users.GetUsersInRoleAsync(rol);
                        foreach (var usuario in usuarios)
                        {
                            if (string.IsNullOrWhiteSpace(usuario.Id) || usuario.IsBanned)
                                continue;

                            destinatariosPorId[usuario.Id] = usuario.Email;
                        }
                    }

                    if (destinatariosPorId.Count == 0)
                    {
                        _logger.LogWarning("HU2: no hay usuarios destinatarios en Bancos para empresa {EmpresaId}", empresaId);
                        return;
                    }

                    string descripcion = $"La documentación obligatoria de la empresa {empresa.RazonSocial} (ID {empresa.Id}) está cargada y disponible para revisión.";
                    string rutaExpediente = $"/Catalogos/Empresas?empresaId={empresa.Id}&seccion=documentos";

                    var notificacion = new NotificacionIntranet
                    {
                        Titulo = "Expediente empresarial completo",
                        Descripcion = descripcion.Length > 500 ? descripcion[..500] : descripcion,
                        Tipo = "EmpresaExpedienteCompleto",
                        Modulo = "Empresas",
                        Url = rutaExpediente,
                        Icono = "bi bi-folder-check",
                        FechaPublicacion = DateTime.Now,
                        Activa = true,
                        UsuariosNotificados = destinatariosPorId.Keys.Select(id => new NotificacionIntranetUsuario
                        {
                            UserId = id,
                            Leida = false,
                            FechaCreacion = DateTime.Now
                        }).ToList()
                    };

                    _db.NotificacionesIntranet.Add(notificacion);
                    estado.FechaUltimaNotificacion = DateTime.UtcNow;

                    correosDestinatarios = destinatariosPorId.Values
                        .Where(email => !string.IsNullOrWhiteSpace(email))
                        .Select(email => email!.Trim())
                        .Where(email => MailAddress.TryCreate(email, out var direccion) &&
                                        string.Equals(direccion.Address, email, StringComparison.OrdinalIgnoreCase))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    razonSocialParaCorreo = empresa.RazonSocial;
                    rutaExpedienteParaCorreo = rutaExpediente;

                    await _db.SaveChangesAsync(ct);
                    notificacionId = notificacion.Id;
                }
                else
                {
                    await _db.SaveChangesAsync(ct);
                }

                estado.FirmaRegla = firma;
                estado.Completo = completo;
                estado.FechaEvaluacion = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }

            // FUERA DE LA TRANSACCIÓN: la caída de Graph no revierte la campanita ni el expediente.
            if (notificacionId != 0 && razonSocialParaCorreo != null && rutaExpedienteParaCorreo != null)
            {
                _logger.LogInformation("HU2: notificación interna {NotificacionId} para empresa {EmpresaId}, {Destinatarios} correos válidos", notificacionId, empresaId, correosDestinatarios.Length);
                await EnviarCorreosAsync(empresaId, razonSocialParaCorreo, rutaExpedienteParaCorreo, correosDestinatarios);
            }
        }

        private async Task EnviarCorreosAsync(int empresaId, string razonSocial, string rutaExpediente, string[] correos)
        {
            if (!_configuration.GetValue<bool>("HU2NotificacionesBancos:EnviarCorreo", false))
            {
                _logger.LogInformation("HU2: campanita creada, correo deshabilitado para empresa {EmpresaId}", empresaId);
                return;
            }

            string? remitente = _configuration["Graph:FromEmail"];
            string? baseUrl = _configuration["Intranet:BaseUrl"];

            if (string.IsNullOrWhiteSpace(remitente) || !MailAddress.TryCreate(remitente, out _) ||
                !Uri.TryCreate(baseUrl, UriKind.Absolute, out var uriBase) ||
                (uriBase.Scheme != Uri.UriSchemeHttps && uriBase.Scheme != Uri.UriSchemeHttp))
            {
                _logger.LogError("HU2: no se pudo enviar correo; verificar Graph:FromEmail e Intranet:BaseUrl");
                return;
            }

            if (correos.Length == 0)
            {
                _logger.LogWarning("HU2: empresa {EmpresaId} sin correos válidos en los usuarios destinatarios", empresaId);
                return;
            }

            // Los valores de la BD se codifican antes de colocarlos en HTML.
            string nombreHtml = HtmlEncoder.Default.Encode(razonSocial);
            var url = new Uri(uriBase, rutaExpediente);
            string urlHtml = HtmlEncoder.Default.Encode(url.AbsoluteUri);
            string body = $@"
<div style='background:#f4f5fb;padding:32px;font-family:Arial,sans-serif;color:#232044'>
  <div style='background:#ffffff;max-width:620px;margin:auto;border-radius:12px;overflow:hidden'>
    <div style='background:#21166f;padding:22px 28px;color:white'>
      <h2 style='margin:0'>Intranet | Expediente documental completo</h2>
    </div>
    <div style='padding:28px'>
      <p>Estimado equipo de Bancos:</p>
      <p>La documentación obligatoria de la empresa <strong>{nombreHtml}</strong> se encuentra cargada al <strong>100%</strong> y está disponible para su revisión en la Intranet.</p>
      <p><strong>Estado:</strong> Pendiente de revisión por Bancos.</p>
      <p style='margin-top:26px'><a href='{urlHtml}' style='background:#21166f;color:white;padding:13px 20px;text-decoration:none;border-radius:8px'>Ver expediente en Intranet</a></p>
      <p style='color:#6b7280;font-size:12px;margin-top:30px'>Aviso automático generado desde Intranet.</p>
    </div>
  </div>
</div>";

            foreach (string correo in correos)
            {
                try
                {
                    var mensaje = new Message
                    {
                        Subject = $"Intranet | Expediente documental completo - {razonSocial}",
                        Body = new ItemBody { ContentType = BodyType.Html, Content = body },
                        ToRecipients = new List<Recipient>
                        {
                            new Recipient { EmailAddress = new EmailAddress { Address = correo } }
                        }
                    };

                    await _graphClient.Users[remitente.Trim()].SendMail.PostAsync(
                        new SendMailPostRequestBody { Message = mensaje, SaveToSentItems = true });
                    _logger.LogInformation("HU2: correo enviado a {Correo} para empresa {EmpresaId}", correo, empresaId);
                }
                catch (Exception ex)
                {
                    // No expone errores del Graph ni bloquea a los demás destinatarios.
                    _logger.LogError(ex, "HU2: error de correo para empresa {EmpresaId} destinatario {Correo}", empresaId, correo);
                }
            }
        }
    }
}
