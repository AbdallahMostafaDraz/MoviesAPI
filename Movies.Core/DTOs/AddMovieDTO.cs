using Microsoft.AspNetCore.Http;
using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.DTOs
{
    public class AddMovieDTO : BaseMovie
    {
        // poster in add new movie must be not nullable
        public IFormFile Poster { get; set; }

    }
}
