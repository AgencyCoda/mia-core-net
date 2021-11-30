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

            return services;
        }
    }
}