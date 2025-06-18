using Microsoft.AspNetCore.Identity.UI.Services;
namespace ERPSEI.Email
{
    public interface IEmailSender
    {
        void SendEmailAsync(string email, string subject, string message);
    }
}
