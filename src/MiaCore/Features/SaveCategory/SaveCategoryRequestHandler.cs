using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.SaveCategory
{
    internal class SaveCategoryRequestHandler : IRequestHandler<SaveCategoryRequest, CategoryDto>
    {
        private readonly IGenericRepository<MiaCategory> _categoryRepository;
        private readonly IMapper _mapper;

        public SaveCategoryRequestHandler(IGenericRepository<MiaCategory> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(SaveCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<MiaCategory>(request);
            category.Slug = "";

            if (!request.Id.HasValue)
                category.Id = await _categoryRepository.InsertAsync(category);

            category.Slug = UrlHelper.GenerateSlug(category.Id + "-" + request.Title);
            await _categoryRepository.UpdateAsync(category);

            var response = _mapper.Map<CategoryDto>(category);
            return response;
        }
    }
}