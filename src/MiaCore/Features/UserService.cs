using System.Threading.Tasks;
using MiaCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MiaCore.Authentication.Login
{
    internal static class UserService
    {
        internal static async Task LoginAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();

            var repo = scope.ServiceProvider.GetService<UserRepository>();

            var request = await context.Request.ReadFromJsonAsync<LoginRequest>();

            var user = await repo.LoginAsync(request.Email, request.Password);

            await context.Response.WriteAsJsonAsync<MiaUser>(user);
        }

        // internal async Task<MiaUser> LoginAsync(LoginRequest command)
        // {
        //     var user = await _userRepository.LoginAsync(command.Email, command.Password);
        //     return user;
        // }

        internal static async Task RegisterAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();

            var request = await context.Request.ReadFromJsonAsync<MiaUser>();

            var repo = scope.ServiceProvider.GetService<UserRepository>();

            request.Id = await repo.InsertAsync(request);

            await context.Response.WriteAsJsonAsync<MiaUser>(request);
        }

        // internal async Task<MiaUser> RegisterAsync(MiaUser request)
        // {
        //     int id = await _userRepository.InsertAsync(request);
        //     request.Id = id;
        //     return request;
        // }

    }
}