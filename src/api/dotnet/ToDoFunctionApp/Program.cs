using System;
using System.IO;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ToDoFunctionApp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureOpenApi()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile(
                            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                            true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton((s) =>
                    {
                        // Use System Managed Identity to get access to the Key Vault
                        SecretClient kvClient = new SecretClient(new Uri(context.Configuration[Constants.kvUri]), new DefaultAzureCredential());
                        Response<KeyVaultSecret> secret = kvClient.GetSecret(context.Configuration[Constants.kvSecretName]);
                        MongoClient client = new MongoClient(secret.Value.Value);
                        return client;
                    });
                })
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();
            
            host.Run();
        }
    }
}