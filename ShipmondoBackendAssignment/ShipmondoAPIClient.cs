using System.Net;
using System.Text;

namespace ShipmondoBackendAssignment;

public class ShipmondoApiClient
{
	private HttpClient httpClient;

	private ShipmondoApiClient(string username, string apiKey)
	{
		httpClient = new HttpClient();
		httpClient.BaseAddress = new Uri("https://sandbox.shipmondo.com/api/public/v3/");
		
		string apiToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + apiKey));
		httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + apiToken);
		httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
	}
	
	public static async Task<ShipmondoApiClient> FromApiCredentialsAsync(string username, string apiKey)
	{
		ShipmondoApiClient client = new(username, apiKey);
		if (!await client.AreCredentialsValidAsync())
		{
			// The client object would be invalid, abort.
			throw new Exception("Credentials are invalid");
		}
		return client;
	}

	/// <summary>
	/// Is the provided credentials valid?
	///
	/// This should be checked just after the client is created, as all subsequent calls would fail if the credentials are invalid.
	/// </summary>
	public async Task<bool> AreCredentialsValidAsync()
	{
		HttpResponseMessage response = await httpClient.GetAsync("account");
		// TODO: We should consider stricter checking here, as the API might return an HTML 404 page with response code 200.
		return response.StatusCode == HttpStatusCode.OK;
	}
}