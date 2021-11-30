using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.UpdateProfile
{
    internal class UpdateProfileRequestHandler : IRequestHandler<UpdateProfileRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;

        public UpdateProfileRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<MiaUserDto> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<MiaUser>(request);

            await _userRepository.UpdateAsync(user);

            var response = _mapper.Map<MiaUserDto>(user);
            return response;
        }
    }
}