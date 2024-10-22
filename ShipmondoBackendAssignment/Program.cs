using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShipmondoBackendAssignment.DB;
using ShipmondoBackendAssignment.Services;
using ShipmondoBackendAssignment.ShipmondoApi;
using AccountBalance = ShipmondoBackendAssignment.DB.Models.AccountBalance;
using Shipment = ShipmondoBackendAssignment.DB.Models.Shipment;

ServiceCollection serviceCollection = new();
serviceCollection.AddLogging(conf => conf.AddConsole().SetMinimumLevel(LogLevel.Debug));
serviceCollection.AddDbContext<ShipmondoDbContext>(conf =>
{
	conf.UseSqlite("Data Source=shipmondo.db");
});
serviceCollection.AddHttpClient<Client>((_, httpClient) =>
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
serviceCollection.AddScoped<AccountService>();
serviceCollection.AddScoped<ShipmentService>();
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

// We use a general logger for the duration of the program.
ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program));

// Check our credentials.
if (!await serviceProvider.GetRequiredService<Client>().AreCredentialsValidAsync())
{
	// The client object would be invalid, abort.
	logger.LogCritical("Credentials are invalid");
	return 1;
}
logger.LogInformation("API credentials are valid!");

// Migrate DB if necessary.
ShipmondoDbContext db = serviceProvider.GetRequiredService<ShipmondoDbContext>();
if ((await db.Database.GetPendingMigrationsAsync()).Any())
{
	logger.LogDebug("Migrations pending, upgrading...");
	await db.Database.MigrateAsync();
	logger.LogDebug("DONE!");
}
else
{
	logger.LogDebug("No pending DB migrations.");
}

// Print latest account balance.
AccountService accountService = serviceProvider.GetRequiredService<AccountService>();
AccountBalance? latestBalance = await accountService.GetLatestLocalBalanceAsync();
if (latestBalance is null)
{
	logger.LogInformation("No locally recorded account balance.");
}
else
{
	logger.LogInformation("Latest account balance: {Balance}", latestBalance);
}

// Update the account balance from the API.
logger.LogInformation("Current balance: {Balance}", await accountService.SaveAccountBalanceLocallyAsync());

// Create a shipment.
ShipmentService shipmentService = serviceProvider.GetRequiredService<ShipmentService>();
logger.LogInformation("Creating shipment...");
Shipment shipment = await shipmentService.CreateShipmentAsync();
logger.LogInformation("Created shipment with id {ShipmentId} and package number {PackageNumber}", shipment.id, shipment.packageNumber);

// Update after shipment is created.
logger.LogInformation("Updated balance: {Balance}", await accountService.SaveAccountBalanceLocallyAsync());

return 0;