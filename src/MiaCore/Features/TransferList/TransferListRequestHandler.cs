using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Features.GenerictList;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.TransferList
{
    internal class TransferListRequestHandler : GenerictListRequestHandler<Transfer>, IRequestHandler<TransferListRequest, object>
    {
        private readonly IGenericRepository<AccountPermission> _accountPermissionRepo;
        private readonly UserHelper _userHelper;
        public TransferListRequestHandler(IGenericRepository<Transfer> repository, IGenericRepository<AccountPermission> accountPermissionRepo, UserHelper userHelper) : base(repository)
        {
            _accountPermissionRepo = accountPermissionRepo;
            _userHelper = userHelper;
        }

        public async Task<object> Handle(TransferListRequest request, CancellationToken cancellationToken)
        {

            var userId = _userHelper.GetUserId();
            var accountPermission = await _accountPermissionRepo.GetByAsync(new Where(nameof(AccountPermission.UserId), userId));

            if (accountPermission is null)
                throw new BadRequestException(ErrorMessages.UserAccountNotFound);

            if (request.Wheres is null)
                request.Wheres = new List<Where>();

            request.Wheres.Add(new Where(nameof(Transfer.AccountId), accountPermission.AccountId));

            return base.Handle(request, cancellationToken);
        }
    }
}