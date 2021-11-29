using System;
using MiaCore.Authentication.Login;
using MiaCore.Mail;
using MiaCore.MApper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace MiaCore.Authentication
{
    public static class Extensions
    {
        public static IServiceCollection AddMiaAuthentication(this IServiceCollection services, Action<MiaCoreOptions> options)
        {
            var opt = new MiaCoreOptions();
            options(opt);

            services.AddOptions<MiaCoreOptions>().Configure(options);
            services.AddScoped<UserRepository>();
            services.AddSendGrid(options =>
                {
                    options.ApiKey = opt.SendgridApiKey;
                });
            services.AddScoped<IMailService, MailService>();
            services.AddAutoMapper(typeof(MiaCoreMappingProfile));

            return services;
        }

        public static IApplicationBuilder UserMiaAuthentication(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("mia-auth/login", UserService.LoginAsync);

                endpoints.MapPost("mia-auth/register", UserService.RegisterAsync);
            });

            return app;
        }
    }
}