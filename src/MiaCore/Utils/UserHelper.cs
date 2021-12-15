using System;
using System.Threading.Tasks;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Utils
{
    internal class UserHelper
    {
        private readonly HttpContext _context;
        private readonly IUserRepository _userRepository;
        public UserHelper(IHttpContextAccessor contextAccessor, IUserRepository userRepository)
        {
            _context = contextAccessor.HttpContext;
            _userRepository = userRepository;
        }
        public int GetUserId()
        {
            if (int.TryParse(_context.User.Identity.Name, out int id))
                return id;

            throw new UnauthorizedException(ErrorMessages.UserIsNotAuthenticated);
        }

        public async Task<MiaUser> GetUserAsync()
        {
            var id = GetUserId();

            var user = await _userRepository.GetAsync(id);

            if (user is null)
                throw new UnauthorizedException(ErrorMessages.UserIsNotAuthenticated);

            return user;
        }
    }
}