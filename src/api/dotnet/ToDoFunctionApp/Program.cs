using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

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
                .ConfigureServices(services =>
                {
                })
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();
            
            

            host.Run();
        }
    }
}