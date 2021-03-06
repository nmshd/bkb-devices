using Azure.Identity;
using Devices.API.ExtensionMethods;
using Devices.Infrastructure.Persistence.Database;
using Enmeshed.Tooling.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;

namespace Devices.API;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build()
            .MigrateDbContext<ApplicationDbContext>((context, _) => { new ApplicationDbContextSeed().SeedAsync(context).Wait(); })
            .MigrateDbContext<ConfigurationDbContext>((context, _) => { new ConfigurationDbContextSeed().SeedAsync(context).Wait(); })
            .Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.AddServerHeader = false;
                options.Limits.MaxRequestBodySize = 1.Kibibytes();
            })
            .ConfigureAppConfiguration(AddAzureAppConfiguration)
            .UseStartup<Startup>();
    }

    private static void AddAzureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder builder)
    {
        var configuration = builder.Build();

        var azureAppConfigurationConfiguration = configuration.GetAzureAppConfigurationConfiguration();

        if (azureAppConfigurationConfiguration.Enabled)
            builder.AddAzureAppConfiguration(appConfigurationOptions =>
            {
                var credentials = new ManagedIdentityCredential();

                appConfigurationOptions
                    .Connect(new Uri(azureAppConfigurationConfiguration.Endpoint), credentials)
                    .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                    .Select("*", "")
                    .Select("*", "Devices");
            });
    }
}
