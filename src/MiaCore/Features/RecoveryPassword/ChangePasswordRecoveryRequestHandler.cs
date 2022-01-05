using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

namespace MiaCore.Features.RecoveryPassword
{
    internal class ChangePasswordRecoveryRequestHandler : IRequestHandler<ChangePasswordRecoveryRequest, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<MiaRecovery> _recoveryRepository;

        public ChangePasswordRecoveryRequestHandler(IUserRepository userRepository, IGenericRepository<MiaRecovery> recoveryRepository)
        {
            _userRepository = userRepository;
            _recoveryRepository = recoveryRepository;
        }

        public async Task<bool> Handle(ChangePasswordRecoveryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return false;

            var recovery = await _recoveryRepository.GetByAsync(new Where(nameof(MiaRecovery.Token), request.Token));
            if (recovery is null || recovery.Status == 1 || recovery.UserId != user.Id)
                return false;

            user.Password = Hashing.GenerateSha256(request.Password);
            await _userRepository.UpdateAsync(user);

            recovery.Status = 1;
            await _recoveryRepository.UpdateAsync(recovery);

            return true;
        }
    }
}