using MediatR;
using MiaCore.Models;

namespace MiaCore.Features.FetchMiaBillingInfo
{
    public class FetchMiaBillingInfoRequest : IRequest<MiaBillingInfo>
    {
        public long Id { get; set; }
    }
}