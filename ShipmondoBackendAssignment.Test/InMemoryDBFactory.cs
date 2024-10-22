using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB;

namespace ShipmondoBackendAssignment.Test;

// Code inspired by https://www.meziantou.net/testing-ef-core-in-memory-using-sqlite.htm

public class InMemoryDBFactory : IDisposable, IAsyncDisposable
{
	private DbConnection? connection;

	private DbContextOptions<ShipmondoDbContext> CreateOptions()
	{
		return new DbContextOptionsBuilder<ShipmondoDbContext>()
			.UseSqlite(connection!)
			.Options;
	}

	public ShipmondoDbContext CreateContext()
	{
		if (connection is null)
		{
			connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			
			DbContextOptions<ShipmondoDbContext> options = CreateOptions();
			using ShipmondoDbContext context = new(options);
			context.Database.EnsureCreated();
		}
		
		return new ShipmondoDbContext(CreateOptions());
	}

	public void Dispose()
	{
		connection?.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (connection != null)
		{
			await connection.DisposeAsync();
		}
	}
}