using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShipmondoBackendAssignment;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.DB.Models;
using ShipmondoBackendAssignment.Services;

ServiceCollection serviceCollection = new();
serviceCollection.AddLogging(conf => conf.AddConsole());
serviceCollection.AddDbContext<ShipmondoDbContext>();
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
serviceCollection.AddScoped<AccountBalanceService>();

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

// Migrate DB if necessary.
ShipmondoDbContext db = serviceProvider.GetService<ShipmondoDbContext>()!;
if ((await db.Database.GetPendingMigrationsAsync()).Any())
{
	logger.LogDebug("Migrations pending...upgrading");
	await db.Database.MigrateAsync();
}
else
{
	logger.LogDebug("No pending DB migrations.");
}

// Print latest account balance.
AccountBalanceService accountBalanceService = serviceProvider.GetRequiredService<AccountBalanceService>();
AccountBalance? latestBalance = await accountBalanceService.GetLatestBalanceAsync();
if (latestBalance is null)
{
	logger.LogInformation("No locally recorded account balance.");
}
else
{
	logger.LogInformation($"Latest account balance: {latestBalance.amount} {latestBalance.currencyCode} ({latestBalance.updateInstant:g})");
}

return 0;