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
        private readonly IMapper _mapper;
        public FetchEntityByIdRequestHandler(IGenericRepository<T> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<object> Handle(FetchEntityByIdRequest<T> request, CancellationToken cancellationToken)
        {
            var res = await _repo.GetAsync(request.Id, request.Withs?.Split(','));

            var returnType = request.GetReturnType();
            if (returnType is not null)
                return _mapper.Map(res, res.GetType(), returnType);

            return res;
        }
    }
}