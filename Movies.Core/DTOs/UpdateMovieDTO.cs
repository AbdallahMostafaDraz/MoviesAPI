using Microsoft.AspNetCore.Http;
using Movies.Core.Models;

namespace Movies.EF.DTOs
{
    public class UpdateMovieDTO : BaseMovie
    {
        // poster in update movie may be nullable
        public IFormFile? Poster { get; set; }

    }
}
