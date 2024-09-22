using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenreRepositry Genre { get; }
        IMovieRepositry Movie { get; }

        Task<int> Complete();

    }
}
