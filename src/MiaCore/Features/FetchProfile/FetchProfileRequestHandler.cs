using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Utils;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.FetchProfile
{
    internal class FetchProfileRequestHandler : IRequestHandler<FetchProfileRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;
        private readonly UserHelper _userHelper;

        public FetchProfileRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options, UserHelper userHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _userHelper = userHelper;
        }

        public async Task<MiaUserDto> Handle(FetchProfileRequest request, CancellationToken cancellationToken)
        {
            long id = _userHelper.GetUserId();
            var user = await _userRepository.GetAsync(id);
            var result = _mapper.Map<MiaUserDto>(user);

            return result;
        }
    }
}