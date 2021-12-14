using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.Register
{
    internal class RegisterRequestHandler : IRequestHandler<RegisterRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;

        public RegisterRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<MiaUserDto> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<MiaUser>(request);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("Email alraedy exists");

            user.Id = await _userRepository.InsertAsync(user);

            var response = _mapper.Map<MiaUserDto>(user);
            return response;
        }
    }
}