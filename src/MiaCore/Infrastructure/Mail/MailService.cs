using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MiaCore.Infrastructure.Mail
{
    public class MailService : IMailService
    {
        private readonly ISendGridClient _client;
        private readonly TemplateBuilder _templateBuilder;
        private readonly MiaCoreOptions _options;
        public MailService(ISendGridClient client, TemplateBuilder templateBuilder, IOptions<MiaCoreOptions> options)
        {
            _client = client;
            _options = options.Value;
            _templateBuilder = templateBuilder;
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

        public async Task SendAsync(string to, string subject, string templateSlug, object args)
        {
            var html = await _templateBuilder.BuildAsync(templateSlug, args);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.EmailFrom, _options.EmailFromName),
                Subject = subject,
                HtmlContent = html
            };
            msg.AddTo(new EmailAddress(to));

            var res = await _client.SendEmailAsync(msg);
        }

        public void SendInBackground(string to, string subject, string templateSlug, object args)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, templateSlug, args));
        }

        public void SendInBackground(string to, string subject, string message)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, message));
        }
    }
}