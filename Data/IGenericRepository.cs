using BG_Tec_Assesment_Minimal_Api.Models;
using System.Linq.Expressions;

namespace BG_Tec_Assesment_Minimal_Api.Data
{
     public interface IGenericRepository<T> where T : class
     {
         Task<T?> GetEntityAsync(Expression<Func<T, bool>> predicate);
         Task<T> AddEntityAsync(T T);
         Task<T?> GetEntityByIdAsync(int entityID);
         Task<List<T>> SearchEntityAsync(Expression<Func<T, bool>> predicate);
         Task SaveChangesAsync();
     }
}
