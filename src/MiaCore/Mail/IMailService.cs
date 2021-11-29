using System.Threading.Tasks;

namespace MiaCore.Mail
{
    public interface IMailService
    {
        Task SendAsync(string to, string subject, string message);
        Task SendAsync(string to, string subject, string fileName, object args);
        void SendInBackground(string to, string subject, string fileName, object args);
        void SendInBackground(string to, string subject, string message);
    }
}
