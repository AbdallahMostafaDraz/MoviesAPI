using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models
{
    public class BaseMovie
    {

        [MaxLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }

        public double Rate { get; set; }
        public string StoryLine { get; set; }
        public byte GenreId { get; set; }
    }
}
