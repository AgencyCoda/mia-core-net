using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.CreateGetAccount
{
    internal class CreateGetAccountRequestHandler : IRequestHandler<CreateGetAccountRequest, Account>
    {
        private readonly IGenericRepository<Account> _accountRepo;
        private readonly IGenericRepository<AccountPermission> _accountPermissionRepo;
        private readonly UserHelper _userHelper;

        public CreateGetAccountRequestHandler(IGenericRepository<Account> accountRepo, IGenericRepository<AccountPermission> accountPermissionRepo, UserHelper userHelper)
        {
            _accountRepo = accountRepo;
            _accountPermissionRepo = accountPermissionRepo;
            _userHelper = userHelper;
        }

        public async Task<Account> Handle(CreateGetAccountRequest request, CancellationToken cancellationToken)
        {
            var userId = _userHelper.GetUserId();
            var perm = await _accountPermissionRepo.GetByAsync(new Where(nameof(AccountPermission.UserId), userId));
            if (perm != null)
                return await _accountRepo.GetAsync(perm.AccountId);

            var account = new Account();
            account.Id = await _accountRepo.InsertAsync(account);
            perm = new AccountPermission
            {
                UserId = userId,
                AccountId = account.Id
            };

            await _accountPermissionRepo.InsertAsync(perm);

            return account;
        }
    }
}