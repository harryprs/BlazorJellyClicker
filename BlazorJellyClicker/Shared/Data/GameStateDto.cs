using BlazorJellyClicker.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Data
{
	public class GameStateDto
	{
		public int Id { get; set; }
		public string StateName { get; set; } = string.Empty;
		public List<Jelly> Jellies { get; set; } = new List<Jelly>(); 
		public double Currency { get; set; }
		public List<BaseUpgrade> ClickPowerUpgrades { get; set; } = new List<BaseUpgrade>();
		public List<MultiUpgrade> ClickMultiUpgrades { get; set; } = new List<MultiUpgrade>();
		public List<MultiUpgrade> IncomeMultiUpgrades { get; set; } = new List<MultiUpgrade>(); 
	}
}
