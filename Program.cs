using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VaultSharp;
// using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1;

var builder = WebApplication.CreateBuilder(args);
// Disable HTTPS redirection
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Only HTTP
});
var app = builder.Build();
const string vaultAddress = "http://172.17.0.2:8200"; // Vault API URL
// const string vaultToken = "root"; // Vault Root Token
const string roleId = "af60b321-aae7-ccc9-56df-d413d7826cf1"; // Replace with fetched Role ID
const string secretId = "5b3f9393-d357-48dc-4d22-5f0a9ad44d9f"; // Replace with fetched Secret ID
const string secretPath = "hello"; // Relative to mount point
const string mountPoint = "secret"; // The KV mount point

// var authMethod = new TokenAuthMethodInfo(vaultToken);
var authMethod = new AppRoleAuthMethodInfo(roleId, secretId);
var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
var vaultClient = new VaultClient(vaultClientSettings);


string secretValue = "";
try
{
    // var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath);
    var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
        // path: secretPath,
        // mountPoint: mountPoint
        path: "hello", // The specific secret path (relative to mount point)
        mountPoint: "secret" // The KV mount point
    );
    secretValue = secret.Data.Data["secret"].ToString();
}
catch (Exception ex)
{
    Console.WriteLine($"Error fetching secret: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    secretValue = "Error retrieving secret!";
}

app.MapGet("/", () => $"Hello World, my secret is: {secretValue}");

app.Run();