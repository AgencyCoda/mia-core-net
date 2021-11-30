using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiaCore.Infrastructure.Persistence
{
    public interface IGenericRepository<T>
    {
        Task<T> GetAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> InsertAsync(T obj);
        Task<bool> UpdateAsync(T obj);
        Task<bool> DeleteAsync(object id);
    }
}