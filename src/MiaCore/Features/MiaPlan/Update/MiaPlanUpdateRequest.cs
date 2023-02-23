using System;
using MediatR;

namespace MiaCore.Features.MiaPlan.Update
{
    public class MiaPlanUpdateRequest : IRequest<Models.MiaPlan>
    {
        public int Id { get; set; }
        public decimal PriceMonth { get; set; }
        public decimal PriceMonthUsd { get; set; }
        public DateTime ChangePriceDate { get; set; }
    }
}
