using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MiaCore.JWT
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add JWT token authentication with Bearer scheme
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secret">Secret</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddMiaAuthentication_(this IServiceCollection services, string secret)
        {
            if (secret is null)
                throw new ArgumentNullException(nameof(secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}