using MediatR;

namespace MiaCore.Features.AssignCategoryToUser
{
    public class AssignCategoryToUserRequest : IRequest<object>
    {
        public int CategoryId { get; set; }
    }
}