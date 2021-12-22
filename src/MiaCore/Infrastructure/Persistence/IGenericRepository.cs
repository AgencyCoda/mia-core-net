using System.Collections.Generic;
using System.Threading.Tasks;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<T> GetAsync(object id);
        Task<T> GetByAsync(params Where[] filters);
        Task<IEnumerable<T>> GetAllAsync();
        Task<GenericListResponse<T>> GetListAsync(string[] relatedEntities, int? limit, int? page, List<Where> wheres, List<Order> orders);
        Task<int> InsertAsync(T obj);
        Task<bool> UpdateAsync(T obj);
        Task<bool> DeleteAsync(object id);
    }
}