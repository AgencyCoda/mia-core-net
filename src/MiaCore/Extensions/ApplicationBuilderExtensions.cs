using MiaCore.Features.CreateUser;
using MiaCore.Features.FetchProfile;
using MiaCore.Features.Firebase;
using MiaCore.Features.GenerictList;
using MiaCore.Features.Login;
using MiaCore.Features.Register;
using MiaCore.Features.UpdateProfile;
using MiaCore.Models;
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
                endpoints.MapPostRequest<RegisterRequest>("mia-auth/register", true);
                endpoints.MapGetRequest<FetchProfileRequest>("mia-auth/me");
                endpoints.MapPostRequest<SaveUserRequest>("mia-auth/user/save");
                endpoints.MapPostRequest<UpdateProfileRequest>("mia-auth/update-profile");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-firebase");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-google");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-facebook");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/recovery");
                endpoints.MapPostRequest<GenerictListRequest<MiaUser>>("user/list");
                endpoints.MapPostRequest<GenerictListRequest<MiaCurrency>>("mia-currency/list", true);
                endpoints.MapPostRequest<GenerictListRequest<MiaCategory>>("mia-category/list", true);
                endpoints.MapPostRequest<GenerictListRequest<News>>("news/list", true);
            });

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return app;
        }
    }
}