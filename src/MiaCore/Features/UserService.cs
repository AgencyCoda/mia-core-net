using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MiaCore.Authentication.Login
{
    internal static class UserService
    {
        internal static async Task LoginAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();

            var repo = scope.ServiceProvider.GetService<UserRepository>();
            var mapper = scope.ServiceProvider.GetService<IMapper>();
            var options = scope.ServiceProvider.GetService<IOptions<MiaCoreOptions>>().Value;

            var request = await context.Request.ReadFromJsonAsync<LoginRequest>();

            var user = await repo.LoginAsync(request.Email, request.Password);
            if (user is null)
                return;

            var response = mapper.Map<LoginResponse>(user);
            response.TokenType = "bearer";
            response.AccessToken = JWT.JwtHelper.GenerateToken(options.JwtSecret, options.TokenExpirationMinutes, user.Id.ToString());

            await context.Response.WriteAsJsonAsync(response);
        }

        internal static async Task RegisterAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();

            var request = await context.Request.ReadFromJsonAsync<MiaUser>();

            var repo = scope.ServiceProvider.GetService<UserRepository>();

            request.Id = await repo.InsertAsync(request);

            await context.Response.WriteAsJsonAsync<MiaUser>(request);
        }
    }
}