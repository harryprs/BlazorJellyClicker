namespace BlazorJellyClicker.Shared.Models
{
    public class Jelly
    {
        public int JellyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double BaseCost { get; set; }
        public double BaseIncome { get; set; }
        public double Cost { get; set; }
        public double Income { get; set; }
        public double IncomeMulti { get; set; }
        public int Count { get; set; }
        public List<MultiUpgrade> MultiUpgrades { get; set; } = new();
        public List<BaseUpgrade> BaseUpgrades { get; set; } = new();
	}
}
