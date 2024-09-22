using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Helpers;
using Movies.Core.DTOs.Authentication;
using Movies.Core.Interfaces;
using Movies.Core.Models.Authentication;
using Movies.EF.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositries
{
    public class AuthRepositry : IAuthRepositry
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTHelper _jWTHelper;
        private IMapper _mapper;

        public AuthRepositry(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<JWTHelper> jWTHelper, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jWTHelper = jWTHelper.Value;
            _mapper = mapper;
        }
        private async Task<JwtSecurityToken> GenerateAccessToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleCliams = new List<Claim>();
            foreach (var role in userRoles)
                roleCliams.Add(new Claim("Rols", role));

            var allClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Union(userClaims)
            .Union(roleCliams);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTHelper.Key));
            var signutre = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jWTHelper.Issuer,
                audience: _jWTHelper.Audince,
                expires: DateTime.UtcNow.AddMinutes(_jWTHelper.DurationInMinutes),
                signingCredentials: signutre
            );
            return token;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(10)
            };

            return refreshToken;
        }

        public async Task<AuthModel> Register(RegisterDTO registerDTO)
        {
            
            AuthModel authModel = new AuthModel();
            if (await _userManager.FindByNameAsync(registerDTO.UserName) is not null)
            {
                authModel.Message = "This Username Is Already Exist!";
                return authModel;
            }

            if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                authModel.Message = "This Email Is Already Exist!";
                return authModel;
            }

            ApplicationUser user = new ApplicationUser();
            _mapper.Map(registerDTO, user);

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                string errors = "";
                foreach (var error in result.Errors)
                {
                    errors += error.Description;
                }
                authModel.Message = errors; 
                return authModel;
            }

            await _userManager.AddToRoleAsync(user, "user");

            var accessToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            authModel.Message = "Success";
            authModel.UserName = user.UserName;
            authModel.Email = user.Email;
            authModel.IsAuthenticated = true;
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authModel.Roles.Add(role);
            }
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
            authModel.AccessTokenExpiration = accessToken.ValidTo;
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;  

            
            return authModel;
        }


        public async Task<AuthModel> GetNewRefreshToken(string currentRefreshToken)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(e => e.RefreshTokens.Any(e => e.Token == currentRefreshToken));

            if (user is null)
            {
                authModel.Message = "Invalid Token!";
                return authModel;
            }

            var refreshTokenFromDB = user.RefreshTokens.Single(e => e.Token == currentRefreshToken);

            if(!refreshTokenFromDB.IsValid)
            {
                authModel.Message = "InActive Token!";
                return authModel;
            }

            refreshTokenFromDB.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            var newAccessToken = await GenerateAccessToken(user);

            authModel.Message = "Genreate New Tokens Successed";
            authModel.Email = user.Email!;
            authModel.UserName = user.UserName!;
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
            authModel.AccessTokenExpiration = newAccessToken.ValidTo;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            authModel.IsAuthenticated = true;
            foreach (var role in await _userManager.GetRolesAsync(user))
                authModel.Roles.Add(role);

            return authModel;
        }

        public async Task<AuthModel> Login(LoginDTO loginDTO)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                authModel.Message = "Invalid Username or Password!";
                return authModel;
            }

            var newAccessToken = await GenerateAccessToken(user);

            if (user.RefreshTokens.Any(e => e.IsValid))
            {
                var activeRefreshToken = user.RefreshTokens.SingleOrDefault(e => e.IsValid);
                authModel.RefreshToken = activeRefreshToken!.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();
                authModel.RefreshToken = newRefreshToken!.Token;
                authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
            }
            authModel.Message = "Login Successed";
            authModel.IsAuthenticated = true;
            authModel.UserName = user.UserName;
            authModel.Email = user.Email;
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
            authModel.AccessTokenExpiration = newAccessToken.ValidTo;
            var userRefreshToken = user.RefreshTokens.Single(e => e.IsValid);
            authModel.RefreshToken = userRefreshToken.Token;
            authModel.RefreshTokenExpiration = userRefreshToken.ExpiresOn; 
            foreach(var role in await _userManager.GetRolesAsync(user)) 
                authModel.Roles.Add(role);
           
            return authModel;
        }

        
        public async Task<string> RevokeRefreshToken(string refreshToken)
        {
           var user = await _userManager.Users.SingleOrDefaultAsync(e => e.RefreshTokens.Any(e => e.Token == refreshToken));

            if (user is null)
                return "Invliad Token!";

            var refreshTokenInDB = user.RefreshTokens.Single(e => e.Token == refreshToken);
            if (!refreshTokenInDB.IsValid)
                return "This Token Is Invalid";

            refreshTokenInDB.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return string.Empty;

        }
    }
}
