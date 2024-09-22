using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Movies.Api.Helpers;
using Movies.Core.Interfaces;
using Movies.Core.Models.Authentication;
using Movies.EF.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositries
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDBContext _context;
        
        public IGenreRepositry Genre { get; private set; }
        public IMovieRepositry Movie { get; private set; }
        public UnitOfWork(AppDBContext context)
        {
            _context = context;
            Genre = new GenreRepoistry(context);
            Movie = new MovieRepositry(context);
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }


        public async ValueTask DisposeAsync()
        {
           await _context.DisposeAsync();
         }
    }
}
