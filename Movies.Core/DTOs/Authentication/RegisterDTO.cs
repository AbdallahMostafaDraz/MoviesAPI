using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.DTOs.Authentication
{
    public class RegisterDTO
    {
        [MaxLength(25)]
        public string FirstName { get; set; }
        [MaxLength(25)]
        public string LastName { get; set; }
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [MaxLength(25)]
        public string UserName { get; set; }

        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [MaxLength(25)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword {  get; set; }

    }
}
