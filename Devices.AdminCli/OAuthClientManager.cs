using Devices.API.Models;
using Enmeshed.StronglyTypedIds;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;

namespace Devices.AdminCli;

public class CreatedClientDTO
{
    public CreatedClientDTO(string clientId, string name, string clientSecret, int accessTokenLifetime)
    {
        ClientId = clientId;
        Name = name;
        ClientSecret = clientSecret;
        AccessTokenLifetime = accessTokenLifetime;
    }

    public string ClientId { get; init; }
    public string Name { get; init; }
    public string ClientSecret { get; init; }
    public int AccessTokenLifetime { get; init; }
}

public class ClientDTO
{
    public ClientDTO(string clientId, string name, int accessTokenLifetime)
    {
        ClientId = clientId;
        Name = name;
        AccessTokenLifetime = accessTokenLifetime;
    }

    public string ClientId { get; set; }
    public string Name { get; set; }
    public int AccessTokenLifetime { get; set; }
}

public class OAuthClientManager
{
    private readonly ConfigurationDbContext _dbContext;

    public OAuthClientManager(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public CreatedClientDTO Create(string? clientId, string? name, string? clientSecret, int? accessTokenLifetime)
    {
        clientSecret = string.IsNullOrEmpty(clientSecret) ? Password.Generate(30) : clientSecret;
        clientId = string.IsNullOrEmpty(clientId) ? ClientIdGenerator.Generate() : clientId;
        accessTokenLifetime ??= 300;
        name = string.IsNullOrEmpty(name) ? clientId : name;

        var client = new Client
        {
            ClientId = clientId,
            ClientName = name,
            ClientSecrets = new List<Secret>
            {
                new(clientSecret.Sha256())
            },
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            AllowAccessTokensViaBrowser = true,
            RequireConsent = false,

            AccessTokenLifetime = accessTokenLifetime.Value,

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                CustomScopes.IdentityResources.IDENTITY_INFORMATION,
                CustomScopes.IdentityResources.DEVICE_INFORMATION,
                CustomScopes.Apis.CHALLENGES,
                CustomScopes.Apis.DEVICES,
                CustomScopes.Apis.MESSAGES,
                CustomScopes.Apis.SYNCHRONIZATION,
                CustomScopes.Apis.FILES,
                CustomScopes.Apis.TOKENS,
                CustomScopes.Apis.RELATIONSHIPS
            }
        };

        _dbContext.Clients.Add(client.ToEntity());
        _dbContext.SaveChanges();

        return new CreatedClientDTO(clientId, name, clientSecret, accessTokenLifetime.Value);
    }

    public void Delete(string clientId)
    {
        var client = _dbContext.Clients.FirstOrDefault(c => c.ClientId == clientId);

        if (client == null)
            throw new Exception($"A client with the client id '{clientId}' does not exist.");

        _dbContext.Clients.Remove(client);
        _dbContext.SaveChanges();
    }

    public IEnumerable<ClientDTO> GetAll()
    {
        var clients = _dbContext.Clients.Select(c => new ClientDTO(c.ClientId, c.ClientName, c.AccessTokenLifetime)).ToList();
        return clients;
    }
}


public static class ClientIdGenerator
{
    public const int MAX_LENGTH = 20;
    public const int PREFIX_LENGTH = 3;
    public const int MAX_LENGTH_WITHOUT_PREFIX = MAX_LENGTH - PREFIX_LENGTH;
    public const string PREFIX = "CLT";

    private static readonly char[] ValidChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public static string Generate()
    {
        var stringValue = StringUtils.Generate(ValidChars, MAX_LENGTH_WITHOUT_PREFIX);
        return PREFIX + stringValue;
    }
}