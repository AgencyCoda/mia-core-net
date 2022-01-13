using System.Collections.Generic;
using System.Threading.Tasks;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    internal interface INewsRepository : IGenericRepository<News>
    {
        Task<List<News>> SearchByLocationAsync(double latitude, double longitude, List<int> categories);
    }
}