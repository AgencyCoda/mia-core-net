using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Mail;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Features.RecoveryPassword
{
    internal class RecoveryPasswordRequestHandler : IRequestHandler<RecoveryPasswordRequest, object>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<MiaRecovery> _recoveryRepository;
        private readonly IMailService _mailService;
        private readonly HttpContext _context;

        public RecoveryPasswordRequestHandler(IUserRepository userRepository, IMailService mailService, IGenericRepository<MiaRecovery> recoveryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _recoveryRepository = recoveryRepository;
            _mailService = mailService;
            _context = httpContextAccessor.HttpContext;
        }

        public async Task<object> Handle(RecoveryPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                throw new BadRequestException(ErrorMessages.EmailNotExists);

            string token = Guid.NewGuid().ToString();
            var recovery = new MiaRecovery
            {
                UserId = user.Id,
                Token = token
            };

            await _recoveryRepository.InsertAsync(recovery);

            // var baseUrl = string.Format("https://{0}{1}", _context.Request.Host, _context.Request.Path);

            await _mailService.SendAsync(user.Email, "Recovery Password", "recovery-password", new { token, email = request.Email, request.BaseUrl });
            return new { Success = true };
        }
    }
}