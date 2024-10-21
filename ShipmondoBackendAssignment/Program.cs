using ShipmondoBackendAssignment;

// Load settings.
const string applicationNamePrefix = "SBA";
const string usernameEnvVarName = applicationNamePrefix + "__username";
const string apiKeyEnvVarName = applicationNamePrefix + "__apikey";

string username = Environment.GetEnvironmentVariable(usernameEnvVarName)
                  ?? throw new Exception($"Please set you Shipmondo username in the \"{usernameEnvVarName}\" environment variable");
string apiKey = Environment.GetEnvironmentVariable(apiKeyEnvVarName)
                  ?? throw new Exception($"Please set you Shipmondo API key in the \"{apiKeyEnvVarName}\" environment variable");

// Configure our Shipmondo API client.
ShipmondoApiClient shipmondoClient = await ShipmondoApiClient.FromApiCredentialsAsync(username, apiKey);
ServiceCollection serviceCollection = new();
serviceCollection.AddLogging(conf => conf.AddConsole());
serviceCollection.AddHttpClient<ShipmondoApiClient>((_, httpClient) =>
{
	httpClient.BaseAddress = new Uri("https://sandbox.shipmondo.com/api/public/v3/");
	httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
		
	// Get configuration from environment variables.
	const string applicationNamePrefix = "SBA";
	const string usernameEnvVarName = applicationNamePrefix + "__username";
	const string apiKeyEnvVarName = applicationNamePrefix + "__apikey";
	string username = Environment.GetEnvironmentVariable(usernameEnvVarName)
	                  ?? throw new Exception($"Please set you Shipmondo username in the \"{usernameEnvVarName}\" environment variable");
	string apiKey = Environment.GetEnvironmentVariable(apiKeyEnvVarName)
	                ?? throw new Exception($"Please set you Shipmondo API key in the \"{apiKeyEnvVarName}\" environment variable");
		
	string apiToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + apiKey));
	httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + apiToken);
});

ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>()!.CreateLogger(typeof(Program));

// Check our credentials.
if (!await serviceProvider.GetRequiredService<ShipmondoApiClient>().AreCredentialsValidAsync())
{
	// The client object would be invalid, abort.
	logger.LogCritical("Credentials are invalid");
	return 1;
}
logger.LogInformation("API credentials are valid!");


return 0;