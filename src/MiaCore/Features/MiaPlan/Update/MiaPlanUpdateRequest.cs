using MediatR;

namespace MiaCore.Features.MiaPlan.Update
{
    public class MiaPlanUpdateRequest : IRequest<Models.MiaPlan>
    {
        public long Id { get; set; }
        public decimal PriceMonth { get; set; }
        public decimal PriceMonthUsd { get; set; }
    }
}
