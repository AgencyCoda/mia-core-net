using MediatR;

namespace MiaCore.Features.AssignCategoryToUser
{
    internal class AssignCategoryToUserRequest : IRequest<object>
    {
        public int CategoryId { get; set; }
    }
}