using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using Movies.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositries
{
    public class GenreRepoistry : GenericRepositry<Genre>, IGenreRepositry
    {
        private AppDBContext _context;
        private DbSet<Genre> _dbSet;
        public GenreRepoistry(AppDBContext context) : base(context)
        { 
            _context = context;
            _dbSet = _context.Set<Genre>(); 
        }

        public async Task<bool> IsValid(byte id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }


    }
}
