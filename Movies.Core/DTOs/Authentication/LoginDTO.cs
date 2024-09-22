using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs.Authentication
{
    public class LoginDTO
    {
        [MaxLength(25)]
        public string UserName { get; set; }
        [MaxLength(25)]
        [DataType(DataType.Password)]   
        public string Password { get; set; }
    }
}
