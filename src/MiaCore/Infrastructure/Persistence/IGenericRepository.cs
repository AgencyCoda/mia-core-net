using System.Collections.Generic;
using System.Threading.Tasks;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Persistence
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<T> GetAsync(object id, string[] relatedEntities = null);
        Task<T> GetByAsync(string[] relatedEntities = null, params Where[] filters);
        Task<T> GetFirstByAsync(string[] relatedEntities = null, params Where[] filters);
        Task<T> GetLastByAsync(string[] relatedEntities = null, params Where[] filters);
        Task<T> GetByAsync(params Where[] filters);
        Task<T> GetFirstByAsync(params Where[] filters);
        Task<T> GetLastByAsync(params Where[] filters);
        Task<IEnumerable<T>> GetAllAsync();
        Task<GenericListResponse<T>> GetListAsync(List<Where> wheres = null, List<Order> orders = null, int? limit = null, int? page = null, string[] relatedEntities = null);
        Task<int> InsertAsync(T obj);
        Task<bool> UpdateAsync(T obj);
        Task<bool> DeleteAsync(object id);
    }
}