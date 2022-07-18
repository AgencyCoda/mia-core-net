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

        public async Task AddCredibilityPointsAsync(long userId, CredibilityPointsChangeReason reason, decimal checkerPoints, decimal creatorPoints)
        {
            var user = await _userRepository.GetAsync(userId);

            int[] adminRoles = { 4, 1, 5 };
            if (adminRoles.Contains(user.Role))
                return;

            var log = new MiaUserCredibilityPointsChangeLog
            {
                UserId = userId,
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

            bool newTransopened = false;
            try
            {
                newTransopened = await _uow.TryBeginTransactionAsync();
                await _logRepository.InsertAsync(log);
                await _userRepository.UpdateAsync(user);

                if (newTransopened)
                    await _uow.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                if (newTransopened)
                    await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task SubtractCredibilityPointsAsync(long userId, CredibilityPointsChangeReason reason, decimal checkerPoints, decimal creatorPoints)
        {
            await AddCredibilityPointsAsync(userId, reason, checkerPoints * -1, creatorPoints * -1);
        }
    }
}