using System.Threading.Tasks;

namespace MiaCore.Infrastructure.Mail
{
    public interface IMailService
    {
        Task SendAsync(string to, string subject, string message);
        Task SendAsync(string to, string subject, string templateSlug, object args);
        void SendInBackground(string to, string subject, string templateSlug, object args);
        void SendInBackground(string to, string subject, string message);
    }
}
