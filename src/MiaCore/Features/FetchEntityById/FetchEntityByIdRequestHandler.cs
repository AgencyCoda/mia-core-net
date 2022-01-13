using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.FetchEntityById
{
    internal class FetchEntityByIdRequestHandler<T> : IRequestHandler<FetchEntityByIdRequest<T>, T> where T : IEntity
    {
        private readonly IGenericRepository<T> _repo;
        private readonly IMapper _mapper;

        public FetchEntityByIdRequestHandler(IGenericRepository<T> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<T> Handle(FetchEntityByIdRequest<T> request, CancellationToken cancellationToken)
        {
            return await _repo.GetAsync(request.Id, request.With);
        }
    }
}