using MediatR;

namespace MiaCore.Features.GeGenerictList
{
    public class GeGenerictListRequest : IRequest<object>
    {
        public string Entity { get; set; }
        public string[] RelatedEntities { get; set; }
        public int Limit { get; set; }
    }
}