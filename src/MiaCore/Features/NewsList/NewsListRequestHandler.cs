using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Features.GenerictList;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.NewsList
{
    internal class NewsListRequestHandler : GenerictListRequestHandler<News>, IRequestHandler<NewsListRequest, object>
    {
        private readonly IGenericRepository<News> _repo;
        private readonly IMapper _mapper;
        public NewsListRequestHandler(IGenericRepository<News> repo, IMapper mapper) : base(repo)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<object> Handle(NewsListRequest request, CancellationToken cancellationToken)
        {
            var list = await base.Handle(request, cancellationToken);
            return _mapper.Map<GenericListResponse<NewsDto>>(list);
        }
    }
}