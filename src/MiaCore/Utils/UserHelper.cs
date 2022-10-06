using System;
using System.Linq;
using System.Threading.Tasks;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Utils
{
    public class UserHelper
    {
        private readonly HttpContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<MiaUser> _userRepository;
        private readonly IGenericRepository<MiaUserCredibilityPointsChangeLog> _logRepository;
        private MiaUser _curentUser;

        public UserHelper(IHttpContextAccessor contextAccessor, IUnitOfWork uow)
        {
            _context = contextAccessor.HttpContext;
            _uow = uow;
            _userRepository = _uow.GetGenericRepository<MiaUser>();
            _logRepository = _uow.GetGenericRepository<MiaUserCredibilityPointsChangeLog>();
        }
        public long GetUserId()
        {
            if (long.TryParse(_context.User.Identity.Name, out long id))
                return id;

            throw new UnauthorizedException(ErrorMessages.UserIsNotAuthenticated);
        }

        public async Task<MiaUser> GetUserAsync()
        {
            if (_curentUser != null)
                return _curentUser;

            var id = GetUserId();

            var user = await _userRepository.GetAsync(id);

            if (user is null)
                throw new UnauthorizedException(ErrorMessages.UserIsNotAuthenticated);

            return _curentUser = user;
        }

        public async Task AddCredibilityPointsAsync(MiaUser user, CredibilityPointsChangeReason reason, decimal checkerPoints, decimal creatorPoints)
        {
            if (user is null)
                return;

            int[] adminRoles = { 4, 1, 5 };
            if (adminRoles.Contains(user.Role))
                return;

            var log = new MiaUserCredibilityPointsChangeLog
            {
                UserId = user.Id,
                CredibilityPointsBefore = user.CredibilityPoints,
                CredibilityPointsCheckerBefore = user.CredibilityPointsChecker,
                CredibilityPointsCreatorBefore = user.CredibilityPointsCreator,
                Reason = reason.ToString()
            };

            user.CredibilityPointsChecker += checkerPoints;
            user.CredibilityPointsCreator += creatorPoints;
            user.CredibilityPoints = Math.Round((user.CredibilityPointsCreator / 100 * 70) + (user.CredibilityPointsChecker / 100 * 30), 2);

            log.CredibilityPointsAfter = user.CredibilityPoints;
            log.CredibilityPointsCheckerAfter = user.CredibilityPointsChecker;
            log.CredibilityPointsCreatorAfter = user.CredibilityPointsCreator;

            await _logRepository.InsertAsync(log);
        }

        public async Task SubtractCredibilityPointsAsync(MiaUser user, CredibilityPointsChangeReason reason, decimal checkerPoints, decimal creatorPoints)
        {
            await AddCredibilityPointsAsync(user, reason, checkerPoints * -1, creatorPoints * -1);
        }
    }
}