using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.CurrentUserPlan
{
    public class CurrentUserPlanRequest : IRequest<MiaUserPlan>
    {
        public string Withs { get; set; }
        public int Id { get; set; }
    }
}