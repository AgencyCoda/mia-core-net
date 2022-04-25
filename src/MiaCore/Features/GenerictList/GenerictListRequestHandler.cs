using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.GenerictList
{
    public class GenerictListRequestHandler<T> : IRequestHandler<GenerictListRequest<T>, object> where T : IEntity
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IMapper _mapper;
        public GenerictListRequestHandler(IGenericRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public virtual async Task<object> Handle(GenerictListRequest<T> request, CancellationToken cancellationToken)
        {
            var res = await _repository.GetListAsync(request.Wheres, request.Orders, request.Limit, request.Page, request.With);

            var returnType = request.GetReturnType();
            if (returnType is not null)
                return _mapper.Map(res, res.GetType(), returnType);

            return res;
        }
    }
}