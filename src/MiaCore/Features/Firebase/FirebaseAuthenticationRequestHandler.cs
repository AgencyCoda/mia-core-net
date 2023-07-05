using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FirebaseAdmin.Auth;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Features.Login;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.Firebase
{
    internal class FirebaseAuthenticationRequestHandler : IRequestHandler<FirebaseAuthenticationRequest, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;

        public FirebaseAuthenticationRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<LoginResponseDto> Handle(FirebaseAuthenticationRequest request, CancellationToken cancellationToken)
        {
            string uid = null;
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.Token);
                uid = decodedToken.Uid;
            }
            catch
            {
                throw new UnauthorizedException(ErrorMessages.InvalidFirebaseToken);
            }
            var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            var user = await _userRepository.GetByEmailAsync(firebaseUser.Email);
            if (user is null)
            {
                user = new MiaUser
                {
                    Email = firebaseUser.Email,
                    Fullname = firebaseUser?.DisplayName,
                    Phone = firebaseUser.PhoneNumber,
                    Photo = firebaseUser.PhotoUrl
                };
                user.Id = await _userRepository.InsertAsync(user);
            }

            var response = _mapper.Map<LoginResponseDto>(user);
            response.TokenType = "bearer";
            response.AccessToken = JWT.JwtHelper.GenerateToken(_options.JwtSecret, _options.TokenExpirationMinutes, user.Id.ToString(), user.Role);

            return response;
        }
    }
}