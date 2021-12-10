using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Devices.Domain.Entities;

public class Identity
{
    public Identity(string? clientId, IdentityAddress address, byte[] publicKey)
    {
        ClientId = clientId;
        Address = address;
        PublicKey = publicKey;
        CreatedAt = SystemTime.UtcNow;
        Devices = new List<Device>();
    }

    public string? ClientId { get; set; }

    public IdentityAddress Address { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Device> Devices { get; set; }

    public bool IsNew()
    {
        return Devices.Count < 1;
    }
}