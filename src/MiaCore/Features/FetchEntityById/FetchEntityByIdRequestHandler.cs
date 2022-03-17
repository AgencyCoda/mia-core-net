using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.FetchEntityById
{
    public class FetchEntityByIdRequestHandler<T> : IRequestHandler<FetchEntityByIdRequest<T>, object> where T : IEntity
    {
        private readonly IGenericRepository<T> _repo;
        public FetchEntityByIdRequestHandler(IGenericRepository<T> repo)
        {
            _repo = repo;
        }

        public async Task<object> Handle(FetchEntityByIdRequest<T> request, CancellationToken cancellationToken)
        {
            return await _repo.GetAsync(request.Id, request.Withs?.Split(','));
        }
    }
}