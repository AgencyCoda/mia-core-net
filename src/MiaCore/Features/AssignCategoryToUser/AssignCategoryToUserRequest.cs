using MediatR;

namespace MiaCore.Features.AssignCategoryToUser
{
    internal class AssignCategoryToUserRequest : IRequest<bool>
    {
        public int CategoryId { get; set; }
    }
}