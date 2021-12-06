using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Utils;

namespace MiaCore.Features.GeGenerictList
{
    public class GeGenerictListRequestHandler : IRequestHandler<GeGenerictListRequest, object>
    {
        public async Task<object> Handle(GeGenerictListRequest request, CancellationToken cancellationToken)
        {
            var entity = string.Concat(request.Entity[0].ToString().ToUpper(), request.Entity.AsSpan(1));
            var name = $"MiaCore.Models.{entity}";
            var type = Type.GetType(name);
            var myObj = Activator.CreateInstance(type);

            return null;
        }
    }
}