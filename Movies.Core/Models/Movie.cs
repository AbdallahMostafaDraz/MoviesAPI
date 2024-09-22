using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Movies.Core.Models
{
    public class Movie : BaseMovie
    {
        public int Id { get; set; }
        public byte[] Poster {  get; set; }
        public Genre Genre { get; set; }
    }
}
