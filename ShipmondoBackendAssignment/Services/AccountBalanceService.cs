using Microsoft.EntityFrameworkCore;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.DB.Models;

namespace ShipmondoBackendAssignment.Services;

public class AccountBalanceService
{
	public async Task<AccountBalance?> GetLatestBalanceAsync()
	{
		await using ShipmondoDbContext db = new();
		
		return await db.AccountBalances.OrderByDescending(it => it.updateInstant).FirstOrDefaultAsync();
	}
}