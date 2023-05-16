using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Models
{
	public class JellyState
	{
		public int JellyId { get; set; }
		public int Count { get; set; }
		public List<UpgradeState> MultiUpgrades { get; set; } = new();
		public List<UpgradeState> BaseUpgrades { get; set; } = new();
	}
}
