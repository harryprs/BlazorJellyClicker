using BlazorJellyClicker.Server.Data;
using BlazorJellyClicker.Shared.Data;
using BlazorJellyClicker.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlazorJellyClicker.Server.Controllers
{
    [ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class JellyClickerController : ControllerBase
	{
		private readonly DataContext _context;

		public JellyClickerController(DataContext context)
		{
			_context = context;
			context.Database.EnsureCreated();
		}

		[HttpGet("GetStates")]
		public async Task<List<GameState>> GetStates([FromHeader] string authorization)
		{
			var userId = GetUserIdFromJwt(authorization);
			var gameStates = await _context.GameState.Where(gs => gs.UserId == userId).ToListAsync();
			return gameStates;
		}

		[HttpGet("LoadState/{stateId}")]
		public async Task<SavedState> LoadState([FromHeader] string authorization, int stateId)
		{
			var userId = GetUserIdFromJwt(authorization);
			var gameState = await _context.GameState.Where(gs => gs.Id == stateId && gs.UserId == userId).FirstOrDefaultAsync();
            var savedState = JsonSerializer.Deserialize<SavedState>(Encoding.UTF8.GetString(Convert.FromBase64String(gameState.SavedState)));
            return savedState;
        }


        [HttpPost("SaveState")]
		public async Task<ActionResult> SaveState([FromHeader] string authorization, GameStateDto state)
		{
			var userId = GetUserIdFromJwt(authorization);
			List<JellyState> jellyStates = new List<JellyState>();
			foreach(var jelly in state.Jellies)
			{
				jellyStates.Add(new JellyState
				{
					JellyId = jelly.JellyId,
					Count = jelly.Count,
					BaseUpgrades = (from j in jelly.BaseUpgrades
									select new UpgradeState() { Id = j.Id, Purchased = j.Purchased}).ToList(),
					MultiUpgrades = (from j in jelly.MultiUpgrades
									 select new UpgradeState() { Id = j.Id, Purchased = j.Purchased }).ToList()
				});
			};

			SavedState savedState = new SavedState()
			{
				Jellies = jellyStates,
				ClickMultiUpgrades = (from u in state.ClickMultiUpgrades
									  select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				ClickPowerUpgrades = (from u in state.ClickPowerUpgrades
									  select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				IncomeMultiUpgrades = (from u in state.IncomeMultiUpgrades
									   select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				Currency = state.Currency
			};

			var stateBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(savedState)));

			GameState stateToSave = new GameState()
			{
				Id = state.Id,
				StateName = state.StateName,
				SavedState = stateBase64,
				UserId = userId
            };

			_context.Update(stateToSave);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpPost("NewSave")]
		public async Task<ActionResult> NewSave([FromHeader] string authorization, GameStateDto state)
		{
			var userId = GetUserIdFromJwt(authorization);
			List<JellyState> jellyStates = new List<JellyState>();
			foreach (var jelly in state.Jellies)
			{
				jellyStates.Add(new JellyState
				{
					JellyId = jelly.JellyId,
					Count = jelly.Count,
					BaseUpgrades = (from j in jelly.BaseUpgrades
									select new UpgradeState() { Id = j.Id, Purchased = j.Purchased }).ToList(),
					MultiUpgrades = (from j in jelly.MultiUpgrades
									 select new UpgradeState() { Id = j.Id, Purchased = j.Purchased }).ToList()
				});
			}

			SavedState savedState = new SavedState()
			{
				Jellies = jellyStates,
				ClickMultiUpgrades = (from u in state.ClickMultiUpgrades
									  select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				ClickPowerUpgrades = (from u in state.ClickPowerUpgrades
									  select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				IncomeMultiUpgrades = (from u in state.IncomeMultiUpgrades
									   select new UpgradeState() { Id = u.Id, Purchased = u.Purchased }).ToList(),
				Currency = state.Currency
			};
			var stateBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(savedState)));

			GameState stateToSave = new GameState()
			{
				StateName = state.StateName,
				SavedState = stateBase64,
				UserId = userId
			};

			_context.Add(stateToSave);
			await _context.SaveChangesAsync();

			return Ok();
		}

		public int GetUserIdFromJwt(string authorization)
		{
			if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
			{
				var parameter = headerValue.Parameter;
				var handler = new JwtSecurityTokenHandler();
				var jsonToken = handler.ReadJwtToken(parameter);
				var userId = Int32.Parse(jsonToken.Claims.First(claim => claim.Type == "jti").Value);
				return userId;
			}
			return -1;
		}
	}
}
