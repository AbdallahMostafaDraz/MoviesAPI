using Microsoft.EntityFrameworkCore;
using Movies.Core.Constants;
using Movies.Core.Interfaces;
using Movies.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositries
{
    public class GenericRepositry<T> : IGenericRepositry<T> where T : class
    {
        private readonly AppDBContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepositry(AppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T> Add(T item)
        {
            await _dbSet.AddAsync(item);
            return item;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> items)
        {
            await _dbSet.AddRangeAsync(items);
            return items;
        }

        public async Task Delete(T item)
        {
            _dbSet.Remove(item);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null, string? orderByDirection = OrderBy.Ascending,
            string[]? includeWords = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
            {
                if (orderByDirection is null || orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else if (orderByDirection == OrderBy.Descending)
                    query = query.OrderByDescending(orderBy);
                else
                    throw new InvalidOperationException("Please Enter orderByDicrection Correctly (ASC || DES)");

            }
            if (includeWords != null)
            {
                foreach (var word in includeWords)
                    query = query.Include(word);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter, string[]? includeWords = null)
        {
            IQueryable<T> query = _dbSet.Where(filter);

            if (includeWords != null)
            {
                foreach (var word in includeWords)
                    query = query.Include(word);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<T> Update(T item)
        {
           _dbSet.Update(item);
            return item;
        }


    }
}
