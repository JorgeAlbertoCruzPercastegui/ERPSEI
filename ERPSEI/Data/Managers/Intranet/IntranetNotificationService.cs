using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace ERPSEI.Data.Managers.Intranet
{
    public class IntranetNotificationService : IIntranetNotificationService
    {
        private readonly GraphServiceClient _graphClient;

        public IntranetNotificationService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task EnviarNotificacionComunicadoPruebaAsync(string titulo, string? descripcion, string urlDestino)
        {
            string fromEmail = "talentohumanosei@asesorcliente.com";
            string correoPrueba = "jcruz@garantecommercial.com";

            string subject = $"Nuevo comunicado interno: {titulo}";

            string body = $@"
                <div style='background:#0b1440;padding:40px 0;font-family:Arial,sans-serif;'>

                    <table width='100%' cellpadding='0' cellspacing='0'>
                        <tr>
                            <td align='center'>

                                <table width='520' cellpadding='0' cellspacing='0'
                                       style='background:#1c274f;border-radius:14px;padding:30px;color:#ffffff;'>

                                    <!-- TITULO -->
                                    <tr>
                                        <td align='center'>
                                            <h2 style='margin:0;color:#ffffff;font-weight:600;'>
                                                Nuevo comunicado interno
                                            </h2>

                                            <div style='width:60px;height:4px;background:#d4af37;margin:15px auto;border-radius:10px;'></div>
                                        </td>
                                    </tr>

                                    <!-- CONTENIDO -->
                                    <tr>
                                        <td style='text-align:center;padding-top:10px;'>

                                            <p style='margin:0 0 10px 0;font-size:14px;color:#cfd6ff;'>
                                                Se ha publicado un nuevo comunicado en la intranet
                                            </p>

                                            <h3 style='margin:10px 0;color:#ffffff;font-size:20px;'>
                                                {titulo}
                                            </h3>

                                            <p style='font-size:14px;color:#cfd6ff;margin-bottom:25px;'>
                                                {descripcion ?? ""}
                                            </p>

                                            <!-- BOTON -->
                                            <a href='{urlDestino}'
                                               style='display:inline-block;
                                                      padding:12px 22px;
                                                      background:#d4af37;
                                                      color:#0b1440;
                                                      text-decoration:none;
                                                      border-radius:10px;
                                                      font-weight:bold;
                                                      font-size:14px;'>
                                                Ver comunicado
                                            </a>

                                        </td>
                                    </tr>

                                </table>

                                <!-- FOOTER -->
                                <p style='margin-top:15px;font-size:11px;color:#9aa3d1;'>
                                    Correo enviado desde Talento Humano SEI
                                </p>

                            </td>
                        </tr>
                    </table>

                </div>";

            await EnviarCorreoAsync(fromEmail, correoPrueba, subject, body);
        }

        public async Task EnviarNotificacionEventoPruebaAsync(string titulo, string? descripcion, string urlDestino)
        {
            string fromEmail = "talentohumanosei@asesorcliente.com";
            string correoPrueba = "jcruz@garantecommercial.com";

            string subject = $"Nuevo evento: {titulo}";

            string body = $@"
                <div style='background:#0b1440;padding:40px 0;font-family:Arial,sans-serif;'>

                    <table width='100%' cellpadding='0' cellspacing='0'>
                        <tr>
                            <td align='center'>

                                <table width='520' cellpadding='0' cellspacing='0'
                                       style='background:#1c274f;border-radius:14px;padding:30px;color:#ffffff;'>

                                    <tr>
                                        <td align='center'>
                                            <h2 style='margin:0;color:#ffffff;font-weight:600;'>
                                                Nuevo evento en la intranet
                                            </h2>

                                            <div style='width:60px;height:4px;background:#d4af37;margin:15px auto;border-radius:10px;'></div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style='text-align:center;padding-top:10px;'>

                                            <p style='margin:0 0 10px 0;font-size:14px;color:#cfd6ff;'>
                                                Se ha publicado un nuevo evento
                                            </p>

                                            <h3 style='margin:10px 0;color:#ffffff;font-size:20px;'>
                                                {titulo}
                                            </h3>

                                            <p style='font-size:14px;color:#cfd6ff;margin-bottom:25px;'>
                                                {descripcion ?? ""}
                                            </p>

                                            <a href='{urlDestino}'
                                               style='display:inline-block;
                                                      padding:12px 22px;
                                                      background:#d4af37;
                                                      color:#0b1440;
                                                      text-decoration:none;
                                                      border-radius:10px;
                                                      font-weight:bold;
                                                      font-size:14px;'>
                                                Ver evento
                                            </a>

                                        </td>
                                    </tr>

                                </table>

                                <p style='margin-top:15px;font-size:11px;color:#9aa3d1;'>
                                    Correo enviado desde Talento Humano SEI
                                </p>

                            </td>
                        </tr>
                    </table>

                </div>";

            await EnviarCorreoAsync(fromEmail, correoPrueba, subject, body);
        }

        private async Task EnviarCorreoAsync(string fromEmail, string toEmail, string subject, string htmlBody)
        {
            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlBody
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toEmail
                        }
                    }
                }
            };

            await _graphClient.Users[fromEmail].SendMail.PostAsync(
                new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                });
        }
    }
}