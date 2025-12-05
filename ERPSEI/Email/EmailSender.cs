using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ERPSEI.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly string mailAddress;
        private readonly string mailPassword;
        private readonly string smtpServer;
        private readonly int smtpPort;

        public EmailSender(string _mailAddress, string _mailPassword, string _smtpServer, int _smtpPort) { 
            mailAddress = _mailAddress;
            mailPassword = _mailPassword;
            smtpServer = _smtpServer;
            smtpPort = _smtpPort;
        }

        public void SendEmailAsync(string email, string subject, string message)
        {
            using (var msg = new MimeMessage())
            {
                msg.From.Add(new MailboxAddress(mailAddress, mailAddress));
                msg.To.Add(new MailboxAddress(email, email));
                msg.Subject = subject;
                msg.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

                using (var client = new SmtpClient())
                {
                    // opcional: desactivar validación de certificado
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // conexión recomendada para O365
                    client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                    // quitar XOAUTH2 para forzar usuario/contraseña
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // usar correo + contraseña de aplicación
                    client.Authenticate(mailAddress, mailPassword);

                    client.Send(msg);
                    client.Disconnect(true);
                }
            }
        }
        /*public void SendEmailAsync(string email, string subject, string message)
        {
            using (MimeMessage msg = new MimeMessage())
            {
                msg.From.Add(new MailboxAddress(mailAddress, mailAddress));
                msg.To.Add(new MailboxAddress(email, email));
                msg.Subject = subject;
                msg.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };
                //msg.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };
                using (var client = new SmtpClient())
                {
                    //Esta instrucción elimina la validación del certificado del servidor de correos.
                    //Esto es debido a que los servidores de correo utilizarán certificados autofirmados
                    //en lugar de utilizar un certificado firmado por una autoridad certificadora confiable.
                    //Otro problema potencial es cuando el software antivirus instalado localmente reemplaza
                    //el certificado para escanear el tráfico web en busca de virus.
                    //En un escenario donde el certificado del servidor se encuentre correcto, esta instrucción deberá eliminarse.

                    // Conexión recomendada para Office 365
                    client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);

                    // Necesario cuando usamos App Password
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Autenticación normal con contraseña de aplicación
                    client.Authenticate(mailAddress, mailPassword);

                    client.Send(msg);
                    client.Disconnect(true);

                    // OPCIONAL: si quieres dejar la validación estricta, elimina esta línea
                    //client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    //  STARTTLS correcto para Office 365
                    //client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                    //client.Authenticate(mailAddress, mailPassword);
                    //client.Send(msg);
                    //client.Disconnect(true);
                    /*client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(smtpServer, smtpPort, true);
                    client.Authenticate(mailAddress, mailPassword);
                    client.Send(msg);
                    client.Disconnect(true);
                }   
            }
        }*/
    }
}
