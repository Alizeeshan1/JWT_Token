using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT_Token.Models;

namespace JWT_Token.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("loginDefaultJwt")]
        public IActionResult LoginDefaultJwt([FromBody] User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@1"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:7159/",
                audience: "https://localhost:7159/",
                claims: new List<Claim>() { new Claim(ClaimTypes.Name, user.Username ?? string.Empty) },
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("loginSecondJwt")]
        public IActionResult LoginSecondJwt([FromBody] User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@2"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:7159/",
                audience: "https://localhost:7159/",
                claims: new List<Claim>() { new Claim(ClaimTypes.Name, user.Username ?? string.Empty) },
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("loginCookie")]
        public async Task<IActionResult> LoginCookie([FromBody] User user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username ?? string.Empty));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(30)
                });

            return Ok();
        }

        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            return Forbid();
        }
    }
}

//git hub