using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.AssignCategoryToUser
{
    internal class AssignCategoryToUserRequestHandler : IRequestHandler<AssignCategoryToUserRequest, object>
    {
        private readonly IGenericRepository<MiaCategory> _categoryRepository;
        private readonly IGenericRepository<MiaUserCategory> _userCategoryRepository;
        private readonly UserHelper _userHelper;

        public AssignCategoryToUserRequestHandler(IGenericRepository<MiaCategory> categoryRepository, IGenericRepository<MiaUserCategory> userCategoryRepository, UserHelper userHelper)
        {
            _categoryRepository = categoryRepository;
            _userCategoryRepository = userCategoryRepository;
            _userHelper = userHelper;
        }

        public async Task<object> Handle(AssignCategoryToUserRequest request, CancellationToken cancellationToken)
        {
            var userId = _userHelper.GetUserId();
            if (await _categoryRepository.GetAsync(request.CategoryId) is null)
                throw new BadRequestException(ErrorMessages.CategoryNotFound);

            var existing = await _userCategoryRepository.GetByAsync(
                new Where(nameof(MiaUserCategory.UserId), userId),
                new Where(nameof(MiaUserCategory.CategoryId), request.CategoryId)
                );

            if (existing != null)
                return new { Success = true };

            var userCategory = new MiaUserCategory
            {
                UserId = userId,
                CategoryId = request.CategoryId
            };

            await _userCategoryRepository.InsertAsync(userCategory);

            return new { Success = true };
        }
    }
}