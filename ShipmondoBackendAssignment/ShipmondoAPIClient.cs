using System.Net;
using System.Text;

namespace ShipmondoBackendAssignment;

public class ShipmondoApiClient(HttpClient httpClient)
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
}