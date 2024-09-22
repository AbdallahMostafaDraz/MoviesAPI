using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.DTOs
{
    public class AddGenreDTO
    {
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
