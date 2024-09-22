using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.DTOs.Authentication;
using Movies.Core.Interfaces;
using Movies.EF.DTOs.Authentication;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthRepositry _authRepositry;

        public AccountController(IAuthRepositry authRepositry)
        {
            _authRepositry = authRepositry;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var authModel = await _authRepositry.Register(registerDTO);
                return Ok(authModel);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetNewRefreshToken")]
        public async Task<IActionResult> GetNewRefreshToken(string currentRefreshToke)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepositry.GetNewRefreshToken(currentRefreshToke);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            return BadRequest(currentRefreshToke);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepositry.Login(loginDTO);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Revoke")]
        public async Task<IActionResult> RevokeRefreshToken(string refreshToken)
        {
            if (ModelState.IsValid)
            {
                var result = await _authRepositry.RevokeRefreshToken(refreshToken);
                if (!string.IsNullOrEmpty(result))
                    return BadRequest(result);
                return Ok("Token Revoked Successfully");
            }
            return BadRequest(ModelState);
        }
    }
}
