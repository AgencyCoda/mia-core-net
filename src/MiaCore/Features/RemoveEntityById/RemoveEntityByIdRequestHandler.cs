using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.RemoveEntityById
{
    public class RemoveEntityByIdRequestHandler<T> : IRequestHandler<RemoveEntityByIdRequest<T>, object> where T : IEntity
    {
        private readonly IGenericRepository<T> _repository;
        public RemoveEntityByIdRequestHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }
        public virtual async Task<object> Handle(RemoveEntityByIdRequest<T> request, CancellationToken cancellationToken)
        {
            var res = await _repository.DeleteAsync(request.Id);
            return new { Success = res };
        }
    }
}