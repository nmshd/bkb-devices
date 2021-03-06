using System.Reflection;
using System.Text.Json;
using Devices.Infrastructure.Persistence.Database;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.AdminCli;

public class ApplicationConfiguration
{
    public string DbConnectionString { get; set; } = null!;

    public void Validate()
    {
        if (string.IsNullOrEmpty(DbConnectionString))
            throw new Exception($"{nameof(DbConnectionString)} must not be empty.");
    }
}

public class Program
{
    private static OAuthClientManager _oAuthClientManager = null!;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() {WriteIndented = true};

    private static readonly ConsoleMenu Menu = new(new MenuItem[]
    {
        new(1, "Create client", CreateClient),
        new(2, "Create anonymous client", CreateAnonymousClient),
        new(3, "Delete client", DeleteClient),
        new(4, "List clients", ListClients),
        new(5, "Exit", Exit)
    });

    public static int Main(string[] args)
    {
        ApplicationConfiguration configuration;
        try
        {
            configuration = GetConfiguration(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        _oAuthClientManager = serviceProvider.GetRequiredService<OAuthClientManager>();

        Run();

        return 0;
    }

    private static void ConfigureServices(IServiceCollection services, ApplicationConfiguration applicationConfiguration)
    {
        services.AddConfigurationDbContext(options =>
        {
            options.DefaultSchema = "Devices";
            options.ConfigureDbContext = builder =>
            {
                builder.UseSqlServer(applicationConfiguration.DbConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
            };
        });

        services.AddLogging();

        services.AddTransient<OAuthClientManager>();
    }

    private static ApplicationConfiguration GetConfiguration(string[] args)
    {
        var commandLineOptions = new ConfigurationBuilder().AddCommandLine(args, new Dictionary<string, string> {{"-c", "ConfigurationFile"}}).Build();
        var configurationFile = commandLineOptions.GetValue<string>("ConfigurationFile");

        var configurationBuilder =
            new ConfigurationBuilder()
                .AddEnvironmentVariables();

        if (!string.IsNullOrEmpty(configurationFile))
        {
            var fullPathToConfigurationFile = Path.Combine(Environment.CurrentDirectory, configurationFile);
            configurationBuilder = configurationBuilder.AddJsonFile(fullPathToConfigurationFile, true, false);
        }

        configurationBuilder = configurationBuilder.AddCommandLine(args);

        var configuration = configurationBuilder.Build();

        var applicationConfiguration = new ApplicationConfiguration
        {
            DbConnectionString = configuration.GetValue<string>("Database:ConnectionString")
        };

        applicationConfiguration.Validate();

        Console.WriteLine("The following configuration is used: ");
        Console.WriteLine(JsonSerializer.Serialize(applicationConfiguration, _jsonSerializerOptions));

        return applicationConfiguration;
    }

    private static void Run()
    {
        while (true)
        {
            var userChoice = Menu.AskForItemChoice();
            userChoice.Action.Invoke();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void CreateClient()
    {
        var clientId = ConsoleHelpers.ReadOptional("clientId (optional)");
        var clientName = ConsoleHelpers.ReadOptional("displayName (optional)");
        var clientSecret = ConsoleHelpers.ReadOptional("clientSecret (optional)");
        var accessTokenLifetime = ConsoleHelpers.ReadOptionalNumber("accessTokenLifetime (default: 300)", 60, 3_600);

        var createdClient = _oAuthClientManager.Create(clientId, clientName, clientSecret, accessTokenLifetime);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static void CreateAnonymousClient()
    {
        var createdClient = _oAuthClientManager.Create(null, null, null, null);

        Console.WriteLine(JsonSerializer.Serialize(createdClient, _jsonSerializerOptions));
        Console.WriteLine("Please note the secret since you cannot obtain it later.");
    }

    private static void DeleteClient()
    {
        try
        {
            var clientId = ConsoleHelpers.ReadRequired("clientId");
            _oAuthClientManager.Delete(clientId);
            Console.WriteLine($"Successfully deleted client '{clientId}'");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private static void ListClients()
    {
        var clients = _oAuthClientManager.GetAll();

        Console.WriteLine("The following clients are configured:");

        foreach (var client in clients)
        {
            Console.WriteLine(JsonSerializer.Serialize(client, _jsonSerializerOptions));
        }
    }

    private static void Exit()
    {
        Environment.Exit(0);
    }
}
