using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllVillaAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1);
        Task<T> GetVillaAsync(Expression<Func<T, bool>> filter = null, bool isTracked = true, string? includeProperties = null);
        Task CreateVillaAsync(T entity);
        //Task UpdateVillaAsync(T entity);
        Task RemoveVillaAsync(T entity);
        Task SaveAsync();
    }
}
