using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace MiaCore.Features.MiaPlan.Update
{
    public class MiaPlanUpdateRequestHandler : IRequestHandler<MiaPlanUpdateRequest, Models.MiaPlan>
    {
        private readonly IGenericRepository<Models.MiaPlan> _repo;
        public MiaPlanUpdateRequestHandler(IGenericRepository<Models.MiaPlan> repo)
        {
            _repo = repo;
        }

        public async Task<Models.MiaPlan> Handle(MiaPlanUpdateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var miaPlan = await _repo.GetAsync(request.Id);
                if (miaPlan is null)
                    throw new ResourceNotFoundException(nameof(Models.MiaPlan));

                miaPlan.PriceMonth = request.PriceMonth;
                miaPlan.PriceMonthUsd = request.PriceMonthUsd;
                await _repo.UpdateAsync(miaPlan);
                return miaPlan;
            }
            catch
            {
                throw;
            }
        }
    }
}
