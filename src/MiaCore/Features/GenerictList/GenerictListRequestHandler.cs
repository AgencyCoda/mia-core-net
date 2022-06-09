using System;
using System.Collections.Generic;
using System.Linq;
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
            var wheres = request.Wheres;

            //// Concat "Where" and "Wheres" filters
            if (!string.IsNullOrEmpty(request.Where))
            {
                if (wheres is null)
                    wheres = new List<Where>();

                var filtersToAdd = request.Where
                                        .Split(";")
                                        .Select(x =>
                                        {
                                            var split = x.Split(":");
                                            return new Where(split[0], split[1]);
                                        })
                                        .ToList();

                wheres.AddRange(filtersToAdd);
            }


            var res = await _repository.GetListAsync(wheres, request.Orders, request.Limit, request.Page, request.With);

            var returnType = request.GetReturnType();
            if (returnType is not null)
                return _mapper.Map(res, res.GetType(), returnType);

            return res;
        }
    }
}