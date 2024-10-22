using Moq;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.Services;
using ShipmondoBackendAssignment.ShipmondoApi;
using AccountBalance = ShipmondoBackendAssignment.DB.Models.AccountBalance;

namespace ShipmondoBackendAssignment.Test;

public class AccountBalanceTest
{
	[Fact]
	public void SimpleInsert()
	{
		using InMemoryDBFactory dbFactory = new();
		
		decimal amount = 123.45m;
		string currencyCode = "DKK";
		DateTime updateInstant = DateTime.Now;

		using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			db.AccountBalances.Add(new AccountBalance
			{
				amount = amount,
				currencyCode = currencyCode,
				updateInstant = updateInstant
			});
			db.SaveChanges();
		}

		using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			AccountBalance? balance = db.AccountBalances.FirstOrDefault();
			Assert.NotNull(balance);
			Assert.Equal(amount, balance.amount);
			Assert.Equal(currencyCode, balance.currencyCode);
			Assert.Equal(updateInstant, balance.updateInstant);
		}
	}
	
	[Fact]
	public async Task CheckNewestTimestamp()
	{
		await using InMemoryDBFactory dbFactory = new();

		async Task<AccountBalance?> GetBalance(ShipmondoDbContext db)
		{
			// Create a new account service for the given db context.
			return await new AccountService(db, new Mock<IShipmondoApiClient>().Object).GetLatestLocalBalanceAsync();
		}
		
		// 1. First, our account balance should be empty.
		await using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			Assert.Null(await GetBalance(db));
		}
		
		// 2. Inserting multiple should yield the newest.
		DateTime updateInstant = DateTime.Now;
		await using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			db.AccountBalances.Add(new AccountBalance
			{
				amount = 0m,
				currencyCode = "FOO",
				updateInstant = updateInstant
			});
			db.AccountBalances.Add(new AccountBalance
			{
				amount = 0m,
				currencyCode = "FOO",
				updateInstant = updateInstant.AddDays(1)
			});
			await db.SaveChangesAsync();
		}

		await using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			Assert.Equal(updateInstant.AddDays(1), (await GetBalance(db))!.updateInstant);
		}
		
		// 3. If an older balance is inserted, it should be ignored.
		await using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			db.AccountBalances.Add(new AccountBalance
			{
				amount = 0m,
				currencyCode = "FOO",
				updateInstant = updateInstant.AddDays(-1)
			});
			await db.SaveChangesAsync();
		}

		await using (ShipmondoDbContext db = dbFactory.CreateContext())
		{
			Assert.Equal(updateInstant.AddDays(1), (await GetBalance(db))!.updateInstant);
		}
	}
}