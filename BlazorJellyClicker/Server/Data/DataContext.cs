using BlazorJellyClicker.Shared;
using BlazorJellyClicker.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace BlazorJellyClicker.Server.Data
{
    public class DataContext : DbContext
	{
		public DbSet<GameState> GameState { get; set; }
		public DbSet<User> User { get; set; }
		public DataContext() { }

		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}