using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MiaCore.Features.MiaPlan.Update
{
    public class MiaPlanUpdateRequestHandler : IRequestHandler<MiaPlanUpdateRequest, Models.MiaPlan>
    {
        private readonly IUnitOfWork _uow;

        public MiaPlanUpdateRequestHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Models.MiaPlan> Handle(MiaPlanUpdateRequest request, CancellationToken cancellationToken)
        {
            if (request.ChangePriceDate.Date < request.ChangePriceDate.Date)
                throw new BadRequestException(ErrorMessages.PlanPriceChangeDateMustBeGreaterThanToday);

            var plansRepo = _uow.GetGenericRepository<Models.MiaPlan>();
            var scheduleRepo = _uow.GetGenericRepository<Models.MiaPlanSchedule>();

            try
            {
                await _uow.BeginTransactionAsync();
                var miaPlan = await plansRepo.GetAsync(request.Id);
                if (miaPlan is null)
                    throw new ResourceNotFoundException(nameof(Models.MiaPlan));

                if (miaPlan.PriceMonth != request.PriceMonth || miaPlan.PriceMonthUsd != request.PriceMonthUsd)
                {
                    miaPlan.PriceMonth = request.PriceMonth;
                    miaPlan.PriceMonthUsd = request.PriceMonthUsd;
                    // await plansRepo.UpdateAsync(miaPlan);

                    var existing = await scheduleRepo.GetByAsync(
                        new Where(nameof(MiaPlanSchedule.PlanId), miaPlan.Id),
                        new Where(nameof(MiaPlanSchedule.Status), (int)MiaPlanScheduleStatus.Pending)
                        );
                    if (existing is not null)
                    {
                        existing.ChangePriceDate = request.ChangePriceDate;
                        existing.PriceMonth = request.PriceMonth;
                        existing.PriceMonthUsd = request.PriceMonthUsd;
                        await scheduleRepo.UpdateAsync(existing);
                    }
                    else
                    {
                        var schedule = new MiaPlanSchedule
                        {
                            PlanId = miaPlan.Id,
                            ChangePriceDate = request.ChangePriceDate,
                            PriceMonth = request.PriceMonth,
                            PriceMonthUsd = request.PriceMonthUsd
                        };

                        await scheduleRepo.InsertAsync(schedule);
                    }

                    await sendEmails(request.Id, request.ChangePriceDate);

                }

                await _uow.CommitTransactionAsync();

                return miaPlan;
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task sendEmails(int planId, DateTime changeDate)
        {
            var plansRepo = _uow.GetGenericRepository<MiaUserPlan>();
            var emailsRepo = _uow.GetGenericRepository<MiaEmailSent>();
            var templatesRepo = _uow.GetGenericRepository<MiaEmailTemplate>();

            var plans = await plansRepo.GetListAsync(new List<Where>(){
                new(nameof(MiaUserPlan.PlanId),planId),
                new(nameof(MiaUserPlan.Status),(int)MiaCore.Models.Enums.MiaUserPlanStatus.Active)
            }, relatedEntities: new string[] { nameof(MiaUserPlan.User) });

            var templateIdEs = (await templatesRepo.GetByAsync(
                new Where(nameof(MiaEmailTemplate.Slug), "price-updated-es")
            )).Id;

            var templateIdEn = (await templatesRepo.GetByAsync(
                new Where(nameof(MiaEmailTemplate.Slug), "price-updated-en")
            )).Id;

            var emails = plans.Data.Select(x => new MiaEmailSent
            {
                UserId = x.UserId,
                Email = x.User.Email,
                TemplateId = x.User.Language == "en" ? templateIdEn : templateIdEs,
                Data = JsonSerializer.Serialize(new
                {
                    price_update_date = x.User.Language == "en" ? changeDate.ToString("mm/dd/yyyy") : changeDate.ToString("dd/mm/yyyy")
                }),
                Status = (int)MiaCore.Models.Enums.MiaEmailSentStatus.Pending
            });

            await emailsRepo.InsertBatchAsync(emails);
        }
    }
}
