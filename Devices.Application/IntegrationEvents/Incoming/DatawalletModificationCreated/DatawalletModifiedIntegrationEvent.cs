using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated
{
    public class DatawalletModifiedIntegrationEvent : IntegrationEvent
    {
        public IdentityAddress Identity { get; private set; }
        public DeviceId ModifiedByDevice { get; private set; }
    }
}
