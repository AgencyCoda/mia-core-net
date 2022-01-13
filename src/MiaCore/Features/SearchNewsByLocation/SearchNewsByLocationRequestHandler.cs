using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.SearchNewsByLocation
{
    internal class SearchNewsByLocationRequestHandler : IRequestHandler<SearchNewsByLocationRequest, List<News>>
    {
        private readonly INewsRepository _repo;
        public SearchNewsByLocationRequestHandler(INewsRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<News>> Handle(SearchNewsByLocationRequest request, CancellationToken cancellationToken)
        {
            return await _repo.SearchByLocationAsync(request.Latitude, request.Longitude, request.Categories);
        }
    }
}