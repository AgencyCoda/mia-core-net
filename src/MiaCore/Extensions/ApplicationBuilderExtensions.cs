using Microsoft.AspNetCore.Builder;

namespace MiaCore.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMiaCore(this IApplicationBuilder app)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return app;
        }
    }
}