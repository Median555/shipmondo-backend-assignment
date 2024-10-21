using System.Net;
using System.Text.Json;

namespace ShipmondoBackendAssignment.ShipmondoApi;

public class Client(HttpClient httpClient)
{
	/// <summary>
	/// Is the provided credentials valid?
	///
	/// This should be checked just after the client is created, as all subsequent calls would fail if the credentials are invalid.
	/// </summary>
	public async Task<bool> AreCredentialsValidAsync()
	{
		HttpResponseMessage response = await httpClient.GetAsync("account");
		// TODO: We should consider stricter checking here, as the API might return an HTML 404 page with response code 200.
		return response.StatusCode == HttpStatusCode.OK; // We don't care about the response body, just that the request was successful.
	}
	
	public async Task<AccountBalance> GetAccountBalanceAsync()
	{
		HttpResponseMessage response =await httpClient.GetAsync("account/balance");
		response.EnsureSuccessStatusCode();

		JsonSerializerOptions options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
			PropertyNameCaseInsensitive = true,
		};
		
		return await JsonSerializer.DeserializeAsync<AccountBalance>(await response.Content.ReadAsStreamAsync(), options)
			?? throw new Exception("Could not deserialize account balance");
	}
}

/// <summary>
/// Represents the account balance.
/// POCO from https://sandbox.shipmondo.com/api/public/v3/specification#/operations/account_balance_get
/// </summary>
public class AccountBalance
{
	public decimal amount { get; set; } // TODO: Clarify if the "number" type mentioned in the API doc is compatible with decimal.
	public string currencyCode { get; set; }
	
	public DateTime updatedAt { get; set; }
}