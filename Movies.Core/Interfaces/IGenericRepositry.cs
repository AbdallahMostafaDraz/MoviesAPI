using Movies.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
    public interface IGenericRepositry<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? orderBy = null, string? OrderByDirection = OrderBy.Ascending,
         string[]? includeWords = null);
        Task<T> GetOneAsync(Expression<Func<T, bool>> filter, string[]? includeWords = null);

        Task<T> Add(T item);

        Task<IEnumerable<T>> AddRange(IEnumerable<T> items);
        Task<T> Update(T item);
        Task Delete(T item);
    }
}
