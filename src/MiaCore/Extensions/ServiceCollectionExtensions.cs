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
using Microsoft.Extensions.Configuration;
using MiaCore.Features.RemoveEntityById;

namespace MiaCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMiaAuthentication(this IServiceCollection services, IConfigurationSection configSection)
        {
            var opt = configSection.Get<MiaCoreOptions>();
            services.Configure<MiaCoreOptions>(configSection);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
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
                // Generic List
                var requestType = typeof(GenerictListRequest<>).MakeGenericType(type);
                var inter = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(object));
                var impl = typeof(GenerictListRequestHandler<>).MakeGenericType(type);
                services.AddScoped(inter, impl);

                //Generic Remove
                var requestType1 = typeof(RemoveEntityByIdRequest<>).MakeGenericType(type);
                var inter1 = typeof(IRequestHandler<,>).MakeGenericType(requestType1, typeof(object));
                var impl1 = typeof(RemoveEntityByIdRequestHandler<>).MakeGenericType(type);
                services.AddScoped(inter1, impl1);
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