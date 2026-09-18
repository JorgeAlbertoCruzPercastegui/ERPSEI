using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace ERPSEI.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _fromEmail;


        public EmailSender(
            IConfiguration configuration
        )
        {
            string tenantId =
                configuration["Graph:TenantId"]
                ?? throw new ArgumentNullException(
                    "Graph:TenantId"
                );


            string clientId =
                configuration["Graph:ClientId"]
                ?? throw new ArgumentNullException(
                    "Graph:ClientId"
                );


            string clientSecret =
                configuration["Graph:ClientSecret"]
                ?? throw new ArgumentNullException(
                    "Graph:ClientSecret"
                );


            _fromEmail =
                configuration["Graph:FromEmail"]
                ?? throw new ArgumentNullException(
                    "Graph:FromEmail"
                );


            ClientSecretCredential credential =
                new(
                    tenantId,
                    clientId,
                    clientSecret
                );


            _graphClient =
                new GraphServiceClient(
                    credential
                );
        }


        // =========================================================
        // ENVÍO SIMPLE
        // =========================================================

        public async Task SendEmailAsync(
            string email,
            string subject,
            string message
        )
        {
            await SendEmailAsync(
                email,
                subject,
                message,
                Array.Empty<EmailAttachment>()
            );
        }


        // =========================================================
        // ENVÍO CON ADJUNTOS
        // =========================================================

        public async Task SendEmailAsync(
            string email,
            string subject,
            string message,
            IEnumerable<EmailAttachment> attachments
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    email
                )
            )
            {
                throw new ArgumentException(
                    "El destinatario (email) es requerido.",
                    nameof(email)
                );
            }


            Message mailMessage =
                new()
                {
                    Subject =
                        subject
                        ??
                        string.Empty,

                    Body =
                        new ItemBody
                        {
                            ContentType =
                                BodyType.Html,

                            Content =
                                message
                                ??
                                string.Empty
                        },

                    ToRecipients =
                        new List<Recipient>
                        {
                            new Recipient
                            {
                                EmailAddress =
                                    new EmailAddress
                                    {
                                        Address =
                                            email
                                    }
                            }
                        }
                };


            List<Microsoft.Graph.Models.Attachment>
                graphAttachments =
                    new();


            if (
                attachments !=
                null
            )
            {
                foreach (
                    EmailAttachment attachment
                    in attachments
                )
                {
                    if (
                        attachment.ContentBytes ==
                        null
                        ||
                        attachment.ContentBytes.Length ==
                        0
                    )
                    {
                        continue;
                    }


                    graphAttachments.Add(
                        new FileAttachment
                        {
                            OdataType =
                                "#microsoft.graph.fileAttachment",

                            Name =
                                attachment.FileName,

                            ContentType =
                                string.IsNullOrWhiteSpace(
                                    attachment.ContentType
                                )
                                    ? "application/octet-stream"
                                    : attachment.ContentType,

                            ContentBytes =
                                attachment.ContentBytes
                        }
                    );
                }
            }


            if (
                graphAttachments.Count >
                0
            )
            {
                mailMessage.Attachments =
                    graphAttachments;
            }


            SendMailPostRequestBody body =
                new()
                {
                    Message =
                        mailMessage,

                    SaveToSentItems =
                        true
                };


            await _graphClient
                .Users[
                    _fromEmail
                ]
                .SendMail
                .PostAsync(
                    body
                );
        }
    }
}