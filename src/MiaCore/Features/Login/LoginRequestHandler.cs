using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Mail;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models.Enums;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.Login
{
    internal class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;
        private readonly IMailService _mailService;

        public LoginRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options, IMailService mailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _mailService = mailService;
        }

        public async Task<LoginResponseDto> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.LoginAsync(request.Email, request.Password);
            if (user is null)
                throw new UnauthorizedException(ErrorMessages.IncorrectPassword);

            if (user.Status == MiaUserStatus.ValidateEmail)
            {
                await _mailService.SendAsync(
                    user.Email, 
                    "", 
                    "successful-registration-with-validation", 
                    user.Language, 
                    new
                    {
                        backUrl = _options.BackUrl, 
                        token = System.Web.HttpUtility.UrlEncode(user.ValidationToken),
                        email = System.Web.HttpUtility.UrlEncode(user.Email)
                    });
                
                throw new UnauthorizedException(ErrorMessages.MailIsNotValidated);
            }
            
            if (user.Status == MiaUserStatus.Blocked)
                throw new UnauthorizedException(ErrorMessages.UserIsBlocked);

            if (user.Status == MiaUserStatus.WaitingForValidation)
                throw new UnauthorizedException(ErrorMessages.WaitingForValidation);

            if (user.Status == MiaUserStatus.RejectedValidation)
                throw new UnauthorizedException(ErrorMessages.RejectedValidation);

            var response = _mapper.Map<LoginResponseDto>(user);
            response.TokenType = "bearer";
            response.AccessToken = JWT.JwtHelper.GenerateToken(_options.JwtSecret, _options.TokenExpirationMinutes, user.Id.ToString(), user.Role);

            return response;
        }
    }
}