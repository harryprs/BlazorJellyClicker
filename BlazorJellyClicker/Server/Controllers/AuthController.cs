using BlazorJellyClicker.Server.Data;
using BlazorJellyClicker.Shared.Data;
using BlazorJellyClicker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BlazorJellyClicker.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly DataContext _context;

		public AuthController(DataContext context)
		{
			_context = context;
		}

		[HttpPost("Register")]
		public async Task<ActionResult<User>> Register(UserDto request)
		{
			request.Username = request.Username.ToLower();
			var existingUser = await _context.User.Where(x => x.Username == request.Username).FirstOrDefaultAsync();

			if (existingUser != null)
			{
				return NotFound();
			}

			// passwordSalt - a random piece of data added to the password before running it through the password hashing algorithm, make it more secure
			// passwordHash - an algorithm which morphs the password into a new string
			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			User user = new User();
			user.Username = request.Username;
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			_context.Add(user);
			await _context.SaveChangesAsync();

			return Ok(user);
		}

		[HttpPost("Login")]
		public async Task<ActionResult<int>> Login(UserDto request)
		{
			request.Username = request.Username.ToLower();
			var dbUser = await _context.User.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
			if (dbUser == null || dbUser.Username != request.Username)
			{
				return NotFound();
			}

			if (!VerifyPasswordHash(request.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
			{
				return NotFound();
			}

			/* Login should contain some JWT auth logic
			 Generate refresh token, append to the User
			 Save User with refresh token to DB
			 Generate access token
			 return Tokens
			*/

			return dbUser.Id;
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
		/*
		// Generate Refresh Token
		private RefreshToken GenerateRefreshToken()
		{
			RefreshToken refreshToken = new RefreshToken();

			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				refreshToken.Token = Convert.ToBase64String(randomNumber);
			}
			refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

			return refreshToken;
		}

		// Generate Access Token
		private string GenerateAccessToken(int userId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			// This key is exposed!! JUST FOR TESTING
			var key = Encoding.ASCII.GetBytes("thisisasecretkeyanddontsharewithanyone");
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, Convert.ToString(userId))
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		*/
	}
}
