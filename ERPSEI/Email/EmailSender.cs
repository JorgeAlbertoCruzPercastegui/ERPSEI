using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System.Net.Mail;

namespace ERPSEI.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _fromEmail;

        public EmailSender(IConfiguration configuration)
        {
            var tenantId = configuration["Graph:TenantId"]
                ?? throw new ArgumentNullException("Graph:TenantId");

            var clientId = configuration["Graph:ClientId"]
                ?? throw new ArgumentNullException("Graph:ClientId");

            var clientSecret = configuration["Graph:ClientSecret"]
                ?? throw new ArgumentNullException("Graph:ClientSecret");

            _fromEmail = configuration["Graph:FromEmail"]
                ?? throw new ArgumentNullException("Graph:FromEmail");

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _graphClient = new GraphServiceClient(credential);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El destinatario (email) es requerido.", nameof(email));

            var mailMessage = new Message
            {
                Subject = subject ?? string.Empty,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = message ?? string.Empty
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = email
                        }
                    }
                }
            };

            var body = new SendMailPostRequestBody
            {
                Message = mailMessage,
                SaveToSentItems = true
            };

            await _graphClient.Users[_fromEmail].SendMail.PostAsync(body);
        }
    }
}