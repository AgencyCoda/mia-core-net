using System;
using System.Threading.Tasks;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Utils
{
    public class UserHelper
    {
        private readonly HttpContext _context;
        private readonly IUserRepository _userRepository;
        private MiaUser _curentUser;

        public UserHelper(IHttpContextAccessor contextAccessor, IUserRepository userRepository)
        {
            _context = contextAccessor.HttpContext;
            _userRepository = userRepository;
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


        public async Task AddUserCredibilityPoints(long userId, decimal points)
        {
            var user = await _userRepository.GetAsync(userId);
            user.CredibilityPoints += points;
            await _userRepository.UpdateAsync(user);
        }

        public async Task SubtractUserCredibilityPoints(long userId, decimal points)
        {
            await AddUserCredibilityPoints(userId, points * -1);
        }
    }
}