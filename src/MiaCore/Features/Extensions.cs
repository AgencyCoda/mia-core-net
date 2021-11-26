using System;
using MiaCore.Authentication.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MiaCore.Authentication
{
    public static class Extensions
    {
        public static IServiceCollection AddMiaAuthentication(this IServiceCollection services, Action<MiaCoreOptions> options)
        {
            services.AddOptions<MiaCoreOptions>().Configure(options);
            services.AddScoped<UserRepository>();
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