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
    public class MovieRepositry : GenericRepositry<Movie>, IMovieRepositry
    {
        public MovieRepositry(AppDBContext context) : base(context) { }
    }
}
