using System.Reflection;
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
                });
            return builder;
        }
    }
}