using Movies.Core.DTOs.Authentication;
using Movies.Core.Models.Authentication;
using Movies.EF.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
    public interface IAuthRepositry
    {
        Task<AuthModel> Register(RegisterDTO registerDTO);
        Task<AuthModel> Login(LoginDTO loginDTO);

        Task<AuthModel> GetNewRefreshToken(string currentRefreshToken);

        Task<string> RevokeRefreshToken(string refreshToken);
    }
}
