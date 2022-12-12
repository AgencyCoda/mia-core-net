using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.CreateUser
{
    internal class SaveUserRequestHandler : IRequestHandler<SaveUserRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;
        private readonly IConfiguration _config;

        public SaveUserRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options, IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _config = config;
        }

        public async Task<MiaUserDto> Handle(SaveUserRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<MiaUser>(request);

            if (!string.IsNullOrEmpty(user.Password))
                user.Password = Hashing.GenerateSha256(user.Password);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != request.Id)
                throw new BadRequestException(ErrorMessages.EmailAlreadyExists);

            if (!request.Id.HasValue)
            {
                var points = _config.GetSection("CredibilityPoints:StartingPoints").Get<decimal>();
                user.CredibilityPointsChecker = points;
                user.CredibilityPointsCreator = points;
                user.CredibilityPoints = points;

                if (user.Language is null)
                    user.Language = "es";

                user.Id = await _userRepository.InsertAsync(user);
            }
            else
            {
                if (string.IsNullOrEmpty(user.Password) && existingUser != null)
                    user.Password = existingUser.Password;

                await _userRepository.UpdateAsync(user);
            }

            var response = _mapper.Map<MiaUserDto>(user);
            return response;
        }
    }
}