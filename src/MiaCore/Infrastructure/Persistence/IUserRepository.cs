using System.Threading.Tasks;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    internal interface IUserRepository : IGenericRepository<MiaUser>
    {
        Task<MiaUser> LoginAsync(string email, string password);
        Task<MiaUser> GetByEmailAsync(string email);
    }
}