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

namespace MiaCore.Features.Register
{
    internal class RegisterRequestHandler : IRequestHandler<RegisterRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;
        private readonly IConfiguration _config;

        public RegisterRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options, IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _config = config;
        }

        public async Task<MiaUserDto> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<MiaUser>(request);
            user.Password = Hashing.GenerateSha256(user.Password);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException(ErrorMessages.EmailAlreadyExists);

            user.Id = await _userRepository.InsertAsync(user);

            var response = _mapper.Map<MiaUserDto>(user);
            return response;
        }
    }
}