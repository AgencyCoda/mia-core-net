using MiaCore.Features.CreateUser;
using MiaCore.Features.FetchProfile;
using MiaCore.Features.Login;
using MiaCore.Features.Register;
using MiaCore.Features.UpdateProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UserMiaAuthentication(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPostRequest<LoginRequest>("mia-auth/login", true);
                endpoints.MapPostRequest<RegisterRequest>("mia-auth/register");
                endpoints.MapPostRequest<FetchProfileRequest>("mia-auth/me");
                endpoints.MapPostRequest<SaveUserRequest>("mia-auth/user/save");
                endpoints.MapPostRequest<UpdateProfileRequest>("mia-auth/update-profile");
            });

            return app;
        }
    }
}