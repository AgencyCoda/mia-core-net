using System;
using System.Reflection;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Infrastructure.Mail;
using MiaCore.Mapper;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using MiaCore.JWT;
using MiaCore.Utils;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MiaCore.Models;
using FluentValidation;
using MiaCore.Features.Register;
using MiaCore.Features.GenerictList;
using System.Linq;

namespace MiaCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMiaAuthentication(this IServiceCollection services, Action<MiaCoreOptions> options)
        {
            var opt = new MiaCoreOptions();
            options(opt);

            services.AddOptions<MiaCoreOptions>().Configure(options);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSendGrid(options =>
                {
                    options.ApiKey = opt.SendgridApiKey;
                });
            services.AddScoped<IMailService, MailService>();
            services.AddAutoMapper(typeof(MiaCoreMappingProfile));
            services.AddMediatR(Assembly.GetExecutingAssembly());


            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IEntity).IsAssignableFrom(p) && !p.IsInterface);
            foreach (var type in types)
            {
                var requestType = typeof(GenerictListRequest<>).MakeGenericType(type);
                var inter = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(object));
                var impl = typeof(GenerictListRequestHandler<>).MakeGenericType(type);
                services.AddScoped(inter, impl);
            }

            services.AddMiaAuthentication_(opt.JwtSecret);
            services.AddHttpContextAccessor();
            services.AddScoped<UserHelper>();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(includeInternalTypes: true);
            services.AddScoped<TemplateBuilder>();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(opt.FirebaseCredentialsFilePath),
            });

            return services;
        }
    }
}