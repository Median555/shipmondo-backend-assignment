using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB.Models;

namespace ShipmondoBackendAssignment.DB;

public class ShipmondoDbContext(DbContextOptions<ShipmondoDbContext> options) : DbContext(options)
{
	public DbSet<Shipment> Shipments { get; set; }
	public DbSet<AccountBalance> AccountBalances { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<AccountBalance>().HasIndex(it => it.updateInstant);
		
		base.OnModelCreating(modelBuilder);
	}
}