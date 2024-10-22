namespace ShipmondoBackendAssignment.ShipmondoApi;

public interface IShipmondoApiClient
{
	/// <summary>
	/// Is the provided credentials valid?
	///
	/// This should be checked just after the client is created, as all subsequent calls would fail if the credentials are invalid.
	/// </summary>
	Task<bool> AreCredentialsValidAsync();

	/// <summary>
	/// Get the current account balance object from the API.
	/// </summary>
	Task<AccountBalance> GetAccountBalanceAsync();

	/// <summary>
	/// Create a new shipment.
	/// </summary>
	Task<Shipment> CreateShipmentAsync();
}

/// <summary>
/// Represents the account balance.
/// POCO of https://sandbox.shipmondo.com/api/public/v3/specification#/operations/account_balance_get
/// </summary>
public class AccountBalance
{
	public decimal amount { get; set; } // TODO: Clarify if the "number" type mentioned in the API doc is compatible with decimal.
	public string currencyCode { get; set; }
	public DateTime updatedAt { get; set; }
}

/// <summary>
/// Represents a shipment, but only the field we need for this assignment.
/// POCO of https://sandbox.shipmondo.com/api/public/v3/specification#/operations/shipments_post
/// </summary>
public class Shipment
{
	public int id { get; set; }
	public string pkgNo { get; set; }
}