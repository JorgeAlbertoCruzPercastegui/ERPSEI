using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ERPSEI.Services.Adquisiciones
{
    public class AdquisicionesEmailService :
        IAdquisicionesEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AdquisicionesEmailService(
            ApplicationDbContext context,
            AppUserManager userManager,
            IEmailSender emailSender,
            IConfiguration configuration
        )
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task NotificarEtapaAprobadaAsync(
            int solicitudId,
            string nombreAprobador,
            int ordenEtapa,
            string nombreEtapa,
            string? siguienteEtapa
        )
        {
            DatosCorreoSolicitud? datos =
                await ObtenerDatosSolicitudAsync(
                    solicitudId
                );

            if (datos == null)
            {
                return;
            }

            string folio =
                WebUtility.HtmlEncode(
                    datos.Folio
                );

            string titulo =
                WebUtility.HtmlEncode(
                    datos.Titulo
                );

            string solicitante =
                WebUtility.HtmlEncode(
                    datos.NombreSolicitante
                );

            string aprobador =
                WebUtility.HtmlEncode(
                    nombreAprobador
                );

            string etapa =
                WebUtility.HtmlEncode(
                    nombreEtapa
                );

            string siguiente =
                WebUtility.HtmlEncode(
                    siguienteEtapa
                    ??
                    string.Empty
                );

            string enlace =
                ConstruirEnlaceSolicitud(
                    solicitudId
                );

            string asunto =
                $"Aprobación registrada | {datos.Folio} | {datos.Titulo}";

            string bloqueSiguiente =
                string.IsNullOrWhiteSpace(
                    siguienteEtapa
                )
                    ? string.Empty
                    : $@"
                        <tr>
                            <td style=""padding:8px 0;color:#6b7280;"">
                                Siguiente etapa
                            </td>
                            <td style=""padding:8px 0;font-weight:600;color:#111827;"">
                                {siguiente}
                            </td>
                        </tr>";

            string html =
                $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""utf-8"" />
</head>
<body style=""margin:0;padding:0;background:#f4f6f8;font-family:Arial,Helvetica,sans-serif;color:#212529;"">

    <div style=""max-width:680px;margin:0 auto;padding:32px 16px;"">

        <div style=""background:#ffffff;border:1px solid #e4e8ef;border-radius:14px;overflow:hidden;"">

            <div style=""padding:24px 28px;background:#21166f;color:#ffffff;"">

                <div style=""font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.08em;opacity:.85;"">
                    ERPSEI · Adquisiciones
                </div>

                <h2 style=""margin:8px 0 0;font-size:22px;font-weight:700;"">
                    Aprobación presupuestal registrada
                </h2>

            </div>

            <div style=""padding:28px;"">

                <p style=""margin:0 0 18px;line-height:1.6;"">
                    Hola <strong>{solicitante}</strong>,
                </p>

                <p style=""margin:0 0 22px;line-height:1.6;color:#4b5563;"">
                    Te informamos que una nueva etapa de aprobación presupuestal
                    de tu solicitud ha sido autorizada y firmada correctamente.
                </p>

                <div style=""padding:18px;margin-bottom:22px;border:1px solid #e5e7eb;border-radius:10px;background:#f9fafb;"">

                    <table style=""width:100%;border-collapse:collapse;font-size:14px;"">

                        <tr>
                            <td style=""padding:8px 0;color:#6b7280;width:38%;"">
                                Solicitud
                            </td>

                            <td style=""padding:8px 0;font-weight:600;color:#111827;"">
                                {folio} - {titulo}
                            </td>
                        </tr>

                        <tr>
                            <td style=""padding:8px 0;color:#6b7280;"">
                                Etapa aprobada
                            </td>

                            <td style=""padding:8px 0;font-weight:600;color:#111827;"">
                                {etapa}
                            </td>
                        </tr>

                        <tr>
                            <td style=""padding:8px 0;color:#6b7280;"">
                                Aprobador
                            </td>

                            <td style=""padding:8px 0;font-weight:600;color:#111827;"">
                                {aprobador}
                            </td>
                        </tr>

                        <tr>
                            <td style=""padding:8px 0;color:#6b7280;"">
                                Avance
                            </td>

                            <td style=""padding:8px 0;font-weight:600;color:#111827;"">
                                {ordenEtapa} de 4 etapas completadas
                            </td>
                        </tr>

                        {bloqueSiguiente}

                    </table>

                </div>

                <div style=""margin:26px 0;text-align:center;"">

                    <a href=""{enlace}""
                       style=""display:inline-block;padding:12px 22px;border-radius:8px;background:#21166f;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;"">
                        Consultar solicitud
                    </a>

                </div>

                <p style=""margin:22px 0 0;color:#6b7280;font-size:13px;line-height:1.5;"">
                    La solicitud continuará automáticamente con el siguiente nivel de autorización.
                </p>

            </div>

            <div style=""padding:16px 28px;background:#f8f9fb;border-top:1px solid #e5e7eb;color:#8a9099;font-size:12px;"">
                Este mensaje fue generado automáticamente por la Intranet ERPSEI.
            </div>

        </div>

    </div>

</body>
</html>";

            await _emailSender.SendEmailAsync(
                datos.Email,
                asunto,
                html
            );
        }

        public async Task NotificarFlujoCompletadoAsync(
            int solicitudId,
            string nombreAprobador,
            IEnumerable<EmailAttachment>? adjuntos = null
        )
        {
            DatosCorreoSolicitud? datos =
                await ObtenerDatosSolicitudAsync(
                    solicitudId
                );

            if (datos == null)
            {
                return;
            }

            string folio =
                WebUtility.HtmlEncode(
                    datos.Folio
                );

            string titulo =
                WebUtility.HtmlEncode(
                    datos.Titulo
                );

            string solicitante =
                WebUtility.HtmlEncode(
                    datos.NombreSolicitante
                );

            string aprobador =
                WebUtility.HtmlEncode(
                    nombreAprobador
                );

            string enlace =
                ConstruirEnlaceSolicitud(
                    solicitudId
                );

            string asunto =
                $"Aprobación presupuestal completada | {datos.Folio} | {datos.Titulo}";

            string html =
                $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""utf-8"" />
</head>
<body style=""margin:0;padding:0;background:#f4f6f8;font-family:Arial,Helvetica,sans-serif;color:#212529;"">

    <div style=""max-width:680px;margin:0 auto;padding:32px 16px;"">

        <div style=""background:#ffffff;border:1px solid #e4e8ef;border-radius:14px;overflow:hidden;"">

            <div style=""padding:24px 28px;background:#21166f;color:#ffffff;"">

                <div style=""font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.08em;opacity:.85;"">
                    ERPSEI · Adquisiciones
                </div>

                <h2 style=""margin:8px 0 0;font-size:22px;"">
                    Aprobación presupuestal completada
                </h2>

            </div>

            <div style=""padding:28px;"">

                <p style=""margin:0 0 18px;line-height:1.6;"">
                    Hola <strong>{solicitante}</strong>,
                </p>

                <p style=""margin:0 0 20px;line-height:1.6;color:#4b5563;"">
                    Te informamos que la solicitud
                    <strong>{folio} - {titulo}</strong>
                    ha completado satisfactoriamente todos los niveles
                    de aprobación presupuestal.
                </p>

                <div style=""margin:22px 0;padding:18px;border:1px solid #cfe8db;border-radius:10px;background:#f2fbf6;"">

                    <div style=""margin-bottom:6px;color:#198754;font-size:16px;font-weight:700;"">
                        ✓ 4 de 4 etapas aprobadas
                    </div>

                    <div style=""color:#52605a;font-size:13px;"">
                        La última autorización fue registrada por {aprobador}.
                    </div>

                </div>

                <p style=""margin:20px 0;line-height:1.6;color:#4b5563;"">
                    Las evidencias de autorización y la documentación disponible
                    han quedado asociadas al expediente de la orden de compra.
                </p>

                <div style=""margin:26px 0;text-align:center;"">

                    <a href=""{enlace}""
                       style=""display:inline-block;padding:12px 22px;border-radius:8px;background:#21166f;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;"">
                        Consultar expediente
                    </a>

                </div>

                <p style=""margin:22px 0 0;color:#6b7280;font-size:13px;line-height:1.5;"">
                    El proceso podrá continuar con las actividades correspondientes
                    de pago y compra.
                </p>

            </div>

            <div style=""padding:16px 28px;background:#f8f9fb;border-top:1px solid #e5e7eb;color:#8a9099;font-size:12px;"">
                Este mensaje fue generado automáticamente por la Intranet ERPSEI.
            </div>

        </div>

    </div>

</body>
</html>";

            List<EmailAttachment> archivos =
                adjuntos?
                    .Where(
                        x =>
                            x.ContentBytes !=
                            null
                            &&
                            x.ContentBytes.Length >
                            0
                    )
                    .ToList()
                ??
                new List<EmailAttachment>();

            if (
                archivos.Count >
                0
            )
            {
                await _emailSender.SendEmailAsync(
                    datos.Email,
                    asunto,
                    html,
                    archivos
                );

                return;
            }

            await _emailSender.SendEmailAsync(
                datos.Email,
                asunto,
                html
            );
        }

        private async Task<DatosCorreoSolicitud?>
            ObtenerDatosSolicitudAsync(
                int solicitudId
            )
        {
            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );

            if (
                solicitud ==
                null
                ||
                string.IsNullOrWhiteSpace(
                    solicitud.UsuarioSolicitanteId
                )
            )
            {
                return null;
            }

            AppUser? usuarioSolicitante =
                await _userManager.FindByIdAsync(
                    solicitud.UsuarioSolicitanteId
                );

            if (
                usuarioSolicitante ==
                null
                ||
                string.IsNullOrWhiteSpace(
                    usuarioSolicitante.Email
                )
            )
            {
                return null;
            }

            string nombreSolicitante =
                usuarioSolicitante.UserName
                ??
                usuarioSolicitante.Email
                ??
                "Usuario";

            return new DatosCorreoSolicitud
            {
                SolicitudId =
                    solicitud.Id,

                Folio =
                    solicitud.Folio,

                Titulo =
                    solicitud.Titulo,

                Email =
                    usuarioSolicitante.Email,

                NombreSolicitante =
                    nombreSolicitante
            };
        }

        private string ConstruirEnlaceSolicitud(
            int solicitudId
        )
        {
            string baseUrl =
                _configuration[
                    "Intranet:BaseUrl"
                ]
                ??
                string.Empty;

            baseUrl =
                baseUrl.TrimEnd(
                    '/'
                );

            return
                $"{baseUrl}/ERP/Adquisiciones?openId={solicitudId}";
        }

        private class DatosCorreoSolicitud
        {
            public int SolicitudId
            {
                get;
                set;
            }

            public string Folio
            {
                get;
                set;
            } = string.Empty;

            public string Titulo
            {
                get;
                set;
            } = string.Empty;

            public string Email
            {
                get;
                set;
            } = string.Empty;

            public string NombreSolicitante
            {
                get;
                set;
            } = string.Empty;
        }
    }
}