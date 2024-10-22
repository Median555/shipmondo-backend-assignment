using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.DB.Models;

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
}