using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.SaveNews
{
    internal class SaveNewsRequestHandler : IRequestHandler<SaveNewsRequest, News>
    {
        private readonly IGenericRepository<News> _repo;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        public SaveNewsRequestHandler(IGenericRepository<News> repo, IMapper mapper, UserHelper userHelper)
        {
            _repo = repo;
            _mapper = mapper;
            _userHelper = userHelper;
        }

        public async Task<News> Handle(SaveNewsRequest request, CancellationToken cancellationToken)
        {
            request.Title = request.Content?.Elements?.FirstOrDefault(x => x.Type == 1)?.Text;
            request.Summary = request.Content?.Elements?.FirstOrDefault(x => x.Type == 2)?.Text;

            var news = _mapper.Map<News>(request);
            news.UserId = _userHelper.GetUserId();
            news.Slug = "";

            if (!request.Id.HasValue)
                news.Id = await _repo.InsertAsync(news);

            news.Slug = UrlHelper.GenerateSlug(news.Id + "-" + request.Title);
            await _repo.UpdateAsync(news);

            return news;
        }
    }
}