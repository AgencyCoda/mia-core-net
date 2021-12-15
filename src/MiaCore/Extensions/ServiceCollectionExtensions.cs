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
            services.AddScoped(typeof(IGenericRepository<>), typeof(BaseRepository<>));
            // services.AddScoped<IGenericRepository<MiaRecovery>, BaseRepository<MiaRecovery>>();
            services.AddSendGrid(options =>
                {
                    options.ApiKey = opt.SendgridApiKey;
                });
            services.AddScoped<IMailService, MailService>();
            services.AddAutoMapper(typeof(MiaCoreMappingProfile));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddMiaAuthentication_(opt.JwtSecret);
            services.AddHttpContextAccessor();
            services.AddScoped<UserHelper>();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(includeInternalTypes: true);
            // services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(opt.FirebaseCredentialsFilePath),
            });

            return services;
        }
    }
}