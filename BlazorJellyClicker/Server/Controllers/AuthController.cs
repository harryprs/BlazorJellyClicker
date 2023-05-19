using BlazorJellyClicker.Server.Data;
using BlazorJellyClicker.Shared.Data;
using BlazorJellyClicker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazorJellyClicker.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _config;

		public AuthController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_config = config;
		}

		[HttpPost("Register")]
		public async Task<ActionResult> Register(UserDto request)
		{
			request.Username = request.Username.ToLower();
			var existingUser = await _context.User.Where(x => x.UserName == request.Username).FirstOrDefaultAsync();

			if (existingUser != null)
			{
				return BadRequest("A User with this name already exists.");
			}

			User user = new User();
			user.UserName = request.Username;
			
			var result = await _userManager.CreateAsync(user, request.Password);
			var succ = result.Succeeded;
			if(result.Succeeded)
			{
				return Ok();
			}
			return BadRequest("Account couldn't be created, please try again later.");
		}

		[HttpPost("Login")]
		public async Task<ActionResult<string>> Login(UserDto request)
		{
			request.Username = request.Username.ToLower();

			var user = await _userManager.FindByNameAsync(request.Username);
			if(user != null)
			{
                var signIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (signIn.Succeeded)
                {
                    string jwt = CreateJWT(user);
                    AppendRefreshTokenCookie(user);
                    return Ok(jwt);
                }
                else
                {
					return BadRequest("Username or Password is wrong.");
                }
            }
			return BadRequest("Username or Password is wrong.");
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

        private void AppendRefreshTokenCookie(User user)
        {
            var options = new CookieOptions();
            options.HttpOnly = true;
            options.Secure = true;
            options.SameSite = SameSiteMode.Strict;
            options.Expires = DateTime.Now.AddMinutes(60);
			HttpContext.Response.Cookies.Append(_config.GetSection("SecretKeys:RefreshKey").Value!, user.SecurityStamp, options);
        }

		public string RefreshToken()
		{
            var cookie = HttpContext.Request.Cookies[_config.GetSection("SecretKeys:RefreshKey").Value!];
            if (cookie != null)
            {
                var user = _userManager.Users.FirstOrDefault(user => user.SecurityStamp == cookie);
                if (user != null)
                {
                    var jwtToken = CreateJWT(user);
					return jwtToken;
                }
                else
                {
					return "";
                }
            }
            else
            {
				return "";
            }
        }

		public string CreateJWT(User user)
		{
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("SecretKeys:JwtKey").Value!));
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName), // NOTE: this will be the "User.Identity.Name" value
				// new Claim(ClaimTypes.Role, "User"),
				//new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7172/",
                audience: "https://localhost:7172/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            var JwtToken = new JwtSecurityTokenHandler().WriteToken(token);
			return JwtToken;
        }
		
    }
}
