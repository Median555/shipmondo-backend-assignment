using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB.Models;

namespace ShipmondoBackendAssignment.DB;

public class ShipmondoDbContext : DbContext
{
	public DbSet<Shipment> Shipments { get; set; }
	public DbSet<AccountBalance> AccountBalances { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<AccountBalance>().HasIndex(it => it.updateInstant);
		
		base.OnModelCreating(modelBuilder);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source=shipmondo.db");
	}
}