using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
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
        private readonly IGenericRepository<MiaEmailSent> _repo;
        private readonly IGenericRepository<MiaUser> _userRepo;
        public MailService(ISendGridClient client, TemplateBuilder templateBuilder, IOptions<MiaCoreOptions> options, IGenericRepository<MiaEmailSent> repo, IGenericRepository<MiaUser> userRepo)
        {
            _client = client;
            _options = options.Value;
            _templateBuilder = templateBuilder;
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task SaveInQueueAsync(long userId, string subject, int templateId, object args)
        {
            var user = await _userRepo.GetAsync(userId);
            if (user is null)
                throw new ResourceNotFoundException("User");

            var item = new MiaEmailSent
            {
                UserId = userId,
                Email = user.Email,
                Subject = subject,
                TemplateId = templateId,
                Data = JsonSerializer.Serialize(args)
            };

            await _repo.InsertAsync(item);
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

        public async Task SendAsync(string to, string subject, string templateSlug, string language, object args)
        {
            templateSlug = _templateBuilder.GetTemplateSlug(templateSlug, language);
            var (title, body) = await _templateBuilder.BuildAsync(templateSlug, args);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.EmailFrom, _options.EmailFromName),
                Subject = title,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress(to));

            var res = await _client.SendEmailAsync(msg);
        }

        public async Task SendAsync(string to, int templateId, object args)
        {
            var (title, body) = await _templateBuilder.BuildAsync(templateId, args);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.EmailFrom, _options.EmailFromName),
                Subject = title,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress(to));

            var res = await _client.SendEmailAsync(msg);
        }

        public void SendInBackground(string to, string subject, string templateSlug, string language, object args)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, templateSlug, language, args));
        }

        public void SendInBackground(string to, string subject, string message)
        {
            _ = Task.Run(async () => await SendAsync(to, subject, message));
        }
    }
}