using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated
{
    public class ExternalEventCreatedIntegrationEvent : IntegrationEvent
    {
        public string EventId { get; private set; }

        public string Owner { get; private set; }
    }
}
