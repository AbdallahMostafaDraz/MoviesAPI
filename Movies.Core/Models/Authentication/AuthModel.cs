using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Authentication
{
    public class AuthModel
    {
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public  List<string> Roles { get; set; } = new List<string>();
        public bool IsAuthenticated { get; set; }
        public string AccessToken { get; set; } 
        public DateTime AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
