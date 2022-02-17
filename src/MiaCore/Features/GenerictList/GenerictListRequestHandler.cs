using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.GenerictList
{
    public class GenerictListRequestHandler<T> : IRequestHandler<GenerictListRequest<T>, object> where T : IEntity
    {
        private readonly IGenericRepository<T> _repository;
        public GenerictListRequestHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }
        public virtual async Task<object> Handle(GenerictListRequest<T> request, CancellationToken cancellationToken)
        {
            var res = await _repository.GetListAsync(request.Wheres, request.Orders, request.Limit, request.Page, request.With);

            return res;
        }
    }
}