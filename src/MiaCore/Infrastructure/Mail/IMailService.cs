using System.Threading.Tasks;

namespace MiaCore.Infrastructure.Mail
{
    public interface IMailService
    {
        Task SendAsync(string to, string subject, string message);
        Task SendAsync(string to, string subject, string templateSlug, string language, object args);
        Task SendAsync(string to, int templateId, object args);
        void SendInBackground(string to, string subject, string templateSlug, string language, object args);
        void SendInBackground(string to, string subject, string message);
        Task SaveInQueueAsync(long userId, string subject, int templateId, object args);
    }
}
