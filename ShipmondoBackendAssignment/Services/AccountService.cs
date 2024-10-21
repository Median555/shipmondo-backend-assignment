using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.ShipmondoApi;
using AccountBalance = ShipmondoBackendAssignment.DB.Models.AccountBalance;

namespace ShipmondoBackendAssignment.Services;

public class AccountService(ShipmondoDbContext db, Client apiClient)
{
	/// <summary>
	/// The latest account balance we have stored locally, or null if none exists.
	/// </summary>
	public async Task<AccountBalance?> GetLatestLocalBalanceAsync()
	{
		await using ShipmondoDbContext db = new();
		
		return await db.AccountBalances.OrderByDescending(it => it.updateInstant).FirstOrDefaultAsync();
	}
	
	/// <summary>
	/// Get the current account balance from the API, and save it locally.
	/// </summary>
	public async Task SaveAccountBalanceLocallyAsync()
	{
		ShipmondoApi.AccountBalance accountBalance = await apiClient.GetAccountBalanceAsync();
		
		await db.AccountBalances.AddAsync(new AccountBalance
		{
			amount = accountBalance.amount,
			currencyCode = accountBalance.currencyCode,
			updateInstant = accountBalance.updatedAt
		});
		await db.SaveChangesAsync();
	}
}