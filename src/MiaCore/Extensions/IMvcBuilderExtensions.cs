using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiaCore.Controllers;
using MiaCore.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace MiaCore.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddMiaCoreControllers(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(Assembly.GetExecutingAssembly())
                .ConfigureApplicationPartManager(mng =>
                {
                    mng.FeatureProviders.Clear();
                    mng.FeatureProviders.Add(new MiaCoreFeatureProvider());
                })
                .AddJsonOptions(op =>
                {
                    var options = new JsonSerializerOptions();
                    var snakeCasePolicy = new SnakeCaseNamingPolicy();
                    op.JsonSerializerOptions.PropertyNamingPolicy = snakeCasePolicy;
                    op.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    // op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(snakeCasePolicy));
                });
            return builder;
        }
    }
}