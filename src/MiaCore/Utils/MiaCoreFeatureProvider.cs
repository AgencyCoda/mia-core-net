using System.Reflection;
using MiaCore;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MiaCore.Utils
{
    public class MiaCoreFeatureProvider : ControllerFeatureProvider
    {
        private MiaCoreOptions _options;
        public MiaCoreFeatureProvider()
        {
            // _options = options;
        }
        protected override bool IsController(TypeInfo typeInfo)
        {
            return base.IsController(typeInfo);
        }
    }
}