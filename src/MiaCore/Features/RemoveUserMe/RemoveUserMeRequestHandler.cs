using MediatR;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace MiaCore.Features.RemoveUserMe
{
    public class RemoveUserMeRequestHandler : IRequestHandler<RemoveUserMeRequest, object>
    {
        private UserHelper _userHelper;
        private IUserRepository _userRepo;
        public RemoveUserMeRequestHandler(UserHelper userHelper, IUserRepository userRepo)
        {
            _userHelper = userHelper;
            _userRepo = userRepo;
        }

        public async Task<object> Handle(RemoveUserMeRequest request, CancellationToken cancellationToken)
        {
            var userId = _userHelper.GetUserId();
            var res = await _userRepo.DeleteAsync(userId);
            return new { Success = res };
        }
    }
}
