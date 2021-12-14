using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Infrastructure.Mail;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Features.RecoveryPassword
{
    internal class RecoveryPasswordRequestHandler : IRequestHandler<RecoveryPasswordRequest, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<MiaRecovery> _recoveryRepository;
        private readonly IMailService _mailService;

        public RecoveryPasswordRequestHandler(IUserRepository userRepository, IGenericRepository<MiaRecovery> recoveryRepository)
        {
            _userRepository = userRepository;
            _recoveryRepository = recoveryRepository;
        }

        public async Task<bool> Handle(RecoveryPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user != null)
                return false;

            string token = Guid.NewGuid().ToString();
            var recovery = new MiaRecovery
            {
                UserId = user.Id,
                Token = token
            };

            string mailBody = $"your recovery token is :{token}";

            await _mailService.SendAsync(user.Email, "Password Recovery", mailBody);
            return true;
        }
    }
}