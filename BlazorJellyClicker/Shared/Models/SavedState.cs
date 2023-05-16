using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Models
{
    public class SavedState
    {
        public List<JellyState> Jellies { get; set; } = new();
        public double Currency { get; set; }
		public List<UpgradeState> ClickPowerUpgrades { get; set; } = new();
		public List<UpgradeState> ClickMultiUpgrades { get; set; } = new();
		public List<UpgradeState> IncomeMultiUpgrades { get; set; } = new();
	}
}
