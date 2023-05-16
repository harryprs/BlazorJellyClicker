using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJellyClicker.Shared.Models
{
	public class GameState
	{
		public int Id { get; set; }
		public string StateName { get; set; } = string.Empty;
		public string SavedState { get; set; } = string.Empty;
		public int UserId { get; set; }
		public User? User { get; set; }
	}
}
