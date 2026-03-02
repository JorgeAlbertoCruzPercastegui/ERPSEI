using System.Threading.Tasks;

namespace ERPSEI.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}