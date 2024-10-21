using System.ComponentModel.DataAnnotations;

namespace ShipmondoBackendAssignment.DB.Models;

public class AccountBalance
{
	// This id is not used by Shipmondo, but is required by EF Core to track the entity.
	[Key]
	public int sequenceId { get; set; }
	
	public decimal amount { get; set; }
	public string currencyCode { get; set; }
	public DateTime updateInstant { get; set; }

	public override string ToString()
	{
		return $"{amount} {currencyCode} ({updateInstant:g})";
	}
}