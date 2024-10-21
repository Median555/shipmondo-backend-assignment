using Microsoft.EntityFrameworkCore;

namespace ShipmondoBackendAssignment.DB.Models;

[Keyless]
public class AccountBalance
{
	public decimal amount { get; set; } // TODO: Clarify if the "number" type mentioned in the API doc is compatible with decimal.
	public string currencyCode { get; set; }
	public DateTime updateInstant { get; set; }
}