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