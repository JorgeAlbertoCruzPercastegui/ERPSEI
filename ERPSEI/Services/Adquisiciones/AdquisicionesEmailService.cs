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
                    Intranet · Adquisiciones
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
                Este mensaje fue generado automáticamente por la Intranet SEI.
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
                    Intranet · Adquisiciones
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
                Este mensaje fue generado automáticamente por la Intranet SEI.
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

        // =========================================================
        // APROBACIÓN PRESUPUESTAL PENDIENTE
        // =========================================================

        public async Task NotificarAprobacionPendienteAsync(
            int solicitudId,
            string usuarioAprobadorId,
            int ordenEtapa,
            string nombreEtapa
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    usuarioAprobadorId
                )
            )
            {
                return;
            }


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
            )
            {
                return;
            }


            AppUser? aprobador =
                await _userManager.FindByIdAsync(
                    usuarioAprobadorId
                );


            if (
                aprobador ==
                null
                ||
                string.IsNullOrWhiteSpace(
                    aprobador.Email
                )
            )
            {
                return;
            }


            AppUser? solicitante =
                null;


            if (
                !string.IsNullOrWhiteSpace(
                    solicitud.UsuarioSolicitanteId
                )
            )
            {
                solicitante =
                    await _userManager.FindByIdAsync(
                        solicitud.UsuarioSolicitanteId
                    );
            }


            AdqAprobacionPresupuestal? aprobacion =
                await _context
                    .AdqAprobacionesPresupuestales
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                    )
                    .OrderByDescending(
                        x =>
                            x.Id
                    )
                    .FirstOrDefaultAsync();


            string nombreAprobador =
                !string.IsNullOrWhiteSpace(
                    aprobador.UserName
                )
                    ? aprobador.UserName
                    : aprobador.Email;


            string nombreSolicitante =
                solicitante != null
                &&
                !string.IsNullOrWhiteSpace(
                    solicitante.UserName
                )
                    ? solicitante.UserName
                    : solicitante?.Email
                        ??
                        "Usuario solicitante";


            decimal monto =
                aprobacion?.MontoSolicitado
                ??
                0;


            string folio =
                WebUtility.HtmlEncode(
                    solicitud.Folio
                );


            string titulo =
                WebUtility.HtmlEncode(
                    solicitud.Titulo
                );


            string aprobadorHtml =
                WebUtility.HtmlEncode(
                    nombreAprobador
                );


            string solicitanteHtml =
                WebUtility.HtmlEncode(
                    nombreSolicitante
                );


            string etapaHtml =
                WebUtility.HtmlEncode(
                    nombreEtapa
                );


            string enlace =
            ConstruirEnlaceAprobacion(
                solicitudId
            );


            string montoTexto =
                monto.ToString(
                    "C2"
                );


            string asunto =
                $"Autorización requerida | {solicitud.Folio} | {solicitud.Titulo}";


            string html =
                $@"
<!DOCTYPE html>
<html lang=""es"">

<head>
    <meta charset=""utf-8"" />
</head>

<body style=""
    margin:0;
    padding:0;
    background:#f4f6f8;
    font-family:Arial,Helvetica,sans-serif;
    color:#212529;
"">

    <div style=""
        max-width:680px;
        margin:0 auto;
        padding:32px 16px;
    "">

        <div style=""
            background:#ffffff;
            border:1px solid #e4e8ef;
            border-radius:14px;
            overflow:hidden;
            box-shadow:0 4px 18px rgba(0,0,0,.04);
        "">

            <!-- HEADER -->
            <div style=""
                padding:26px 30px;
                background:#21166f;
                color:#ffffff;
            "">

                <div style=""
                    font-size:12px;
                    font-weight:700;
                    text-transform:uppercase;
                    letter-spacing:.08em;
                    opacity:.85;
                "">
                    Intranet · Adquisiciones
                </div>

                <h2 style=""
                    margin:8px 0 0;
                    font-size:22px;
                    line-height:1.3;
                "">
                    Autorización presupuestal requerida
                </h2>

            </div>


            <!-- BODY -->
            <div style=""
                padding:30px;
            "">

                <p style=""
                    margin:0 0 18px;
                    line-height:1.6;
                "">
                    Hola <strong>{aprobadorHtml}</strong>,
                </p>


                <p style=""
                    margin:0 0 24px;
                    color:#4b5563;
                    line-height:1.7;
                "">
                    Se te ha asignado una solicitud para revisión,
                    autorización y firma dentro del flujo de aprobación
                    presupuestal de Adquisiciones.
                </p>


                <!-- ALERTA -->
                <div style=""
                    padding:16px 18px;
                    margin-bottom:24px;
                    border:1px solid #d9d5f3;
                    border-radius:10px;
                    background:#f7f6ff;
                "">

                    <div style=""
                        font-size:13px;
                        color:#5b54a4;
                        margin-bottom:4px;
                    "">
                        Etapa que requiere tu autorización
                    </div>

                    <div style=""
                        font-size:17px;
                        font-weight:700;
                        color:#21166f;
                    "">
                        Nivel {ordenEtapa} · {etapaHtml}
                    </div>

                </div>


                <!-- DATOS -->
                <div style=""
                    padding:20px;
                    border:1px solid #e5e7eb;
                    border-radius:10px;
                    background:#fafbfc;
                "">

                    <table style=""
                        width:100%;
                        border-collapse:collapse;
                        font-size:14px;
                    "">

                        <tr>
                            <td style=""
                                width:35%;
                                padding:9px 0;
                                color:#6b7280;
                            "">
                                Folio
                            </td>

                            <td style=""
                                padding:9px 0;
                                font-weight:700;
                                color:#111827;
                            "">
                                {folio}
                            </td>
                        </tr>


                        <tr>
                            <td style=""
                                padding:9px 0;
                                color:#6b7280;
                            "">
                                Solicitud
                            </td>

                            <td style=""
                                padding:9px 0;
                                font-weight:600;
                                color:#111827;
                            "">
                                {titulo}
                            </td>
                        </tr>


                        <tr>
                            <td style=""
                                padding:9px 0;
                                color:#6b7280;
                            "">
                                Solicitante
                            </td>

                            <td style=""
                                padding:9px 0;
                                font-weight:600;
                                color:#111827;
                            "">
                                {solicitanteHtml}
                            </td>
                        </tr>


                        <tr>
                            <td style=""
                                padding:9px 0;
                                color:#6b7280;
                            "">
                                Monto
                            </td>

                            <td style=""
                                padding:9px 0;
                                font-weight:700;
                                color:#111827;
                            "">
                                {montoTexto}
                            </td>
                        </tr>


                        <tr>
                            <td style=""
                                padding:9px 0;
                                color:#6b7280;
                            "">
                                Progreso
                            </td>

                            <td style=""
                                padding:9px 0;
                                font-weight:600;
                                color:#111827;
                            "">
                                Nivel {ordenEtapa} de 4
                            </td>
                        </tr>

                    </table>

                </div>


                <!-- CTA -->
                <div style=""
                    text-align:center;
                    margin:30px 0 24px;
                "">

                    <a href=""{enlace}""
                       style=""
                            display:inline-block;
                            padding:14px 26px;
                            border-radius:8px;
                            background:#0d6efd;
                            color:#ffffff;
                            text-decoration:none;
                            font-size:15px;
                            font-weight:700;
                       "">
                        Revisar y autorizar
                    </a>

                </div>


                <p style=""
                    margin:0;
                    color:#6b7280;
                    font-size:13px;
                    line-height:1.6;
                    text-align:center;
                "">
                    El botón te dirigirá a la Intranet SEI
                    para consultar la orden y registrar tu decisión.
                </p>

            </div>


            <!-- FOOTER -->
            <div style=""
                padding:17px 30px;
                border-top:1px solid #e5e7eb;
                background:#f8f9fb;
                color:#8a9099;
                font-size:12px;
            "">
                Este mensaje fue generado automáticamente por
                el módulo de Adquisiciones de la Intranet SEI.
            </div>

        </div>

    </div>

</body>

</html>";


            await _emailSender.SendEmailAsync(
                aprobador.Email,
                asunto,
                html
            );
        }

        private string ConstruirEnlaceAprobacion(int solicitudId)
        {
            string baseUrl =
                _configuration["Intranet:BaseUrl"]
                ??
                string.Empty;

            baseUrl =
                baseUrl.TrimEnd('/');

            return
                $"{baseUrl}/ERP/Adquisiciones?openAprobacionId={solicitudId}";
        }
    }
}