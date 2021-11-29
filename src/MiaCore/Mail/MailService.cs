using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MiaCore.Mail
{
    public class MailService : IMailService
    {
        private readonly ISendGridClient _client;
        private readonly MiaCoreOptions _options;
        public MailService(ISendGridClient client, IOptions<MiaCoreOptions> options)
        {
            _client = client;
            _options = options.Value;
        }
        public async Task SendAsync(string to, string subject, string message)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.EmailFrom, _options.EmailFromName),
                Subject = subject,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(to));

            var res = await _client.SendEmailAsync(msg);
        }
        public async Task SendAsync(string to, string subject, string fileName, object args)
        {
            var html = await TemplateBuilder.BuildAsync(fileName, args);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.EmailFrom, _options.EmailFromName),
                Subject = subject,
                HtmlContent = html
            };
            msg.AddTo(new EmailAddress(to));

            var res = await _client.SendEmailAsync(msg);
        }

        public void SendInBackground(string to, string subject, string fileName, object args)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, fileName, args));
        }

        public void SendInBackground(string to, string subject, string message)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, message));
        }
    }
}