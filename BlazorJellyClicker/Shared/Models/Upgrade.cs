using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Models
{
	public class Upgrade
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public double Cost { get; set; }
		public bool Purchased { get; set; } = false;
		public int OrderInSeries { get; set; }
	}
}
