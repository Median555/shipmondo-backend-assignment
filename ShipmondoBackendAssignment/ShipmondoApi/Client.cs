using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShipmondoBackendAssignment.ShipmondoApi;

public class Client(HttpClient httpClient)
{
	private readonly JsonSerializerOptions defaultOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		PropertyNameCaseInsensitive = true,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
	};
	
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
	
	/// <summary>
	/// Get the current account balance object from the API.
	/// </summary>
	public async Task<AccountBalance> GetAccountBalanceAsync()
	{
		HttpResponseMessage response = await httpClient.GetAsync("account/balance");
		response.EnsureSuccessStatusCode();
		
		return await JsonSerializer.DeserializeAsync<AccountBalance>(await response.Content.ReadAsStreamAsync(), defaultOptions)
			?? throw new Exception("Could not deserialize account balance");
	}

	
	/// <summary>
	/// Create a new shipment.
	/// </summary>
	public async Task<Shipment> CreateShipmentAsync()
	{
		// For this assignment, we are allowed to use a static dummy body.
		string dummyBody = """
		                   {
		                     "own_agreement": false,
		                     "label_format": "a4_pdf",
		                     "product_code": "GLSDK_SD",
		                     "service_codes": "EMAIL_NT,SMS_NT",
		                     "reference": "Order 10001",
		                     "automatic_select_service_point": true,
		                     "sender": {
		                       "name": "Min Virksomhed ApS",
		                       "attention": "Lene Hansen",
		                       "address1": "Hvilehøjvej 25",
		                       "address2": null,
		                       "zipcode": "5220",
		                       "city": "Odense SØ",
		                       "country_code": "DK",
		                       "email": "info@minvirksomhed.dk",
		                       "mobile": "70400407"
		                     },
		                     "receiver": {
		                       "name": "Lene Hansen",
		                       "attention": null,
		                       "address1": "Skibhusvej 52",
		                       "address2": null,
		                       "zipcode": "5000",
		                       "city": "Odense C",
		                       "country_code": "DK",
		                       "email": "lene@email.dk",
		                       "mobile": "12345678"
		                     },
		                     "parcels": [
		                       {
		                         "weight": 1000
		                       }
		                     ]
		                   }
		                   """;

		HttpResponseMessage response = await httpClient.PostAsync(
			"shipments",
			new StringContent(dummyBody, Encoding.UTF8, "application/json")
		);
		response.EnsureSuccessStatusCode();

		return await JsonSerializer.DeserializeAsync<Shipment>(await response.Content.ReadAsStreamAsync(), defaultOptions)
		       ?? throw new Exception("Could not deserialize shipment");
	}
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