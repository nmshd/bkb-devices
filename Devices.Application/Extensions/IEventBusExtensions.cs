using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;
using Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

namespace Devices.Application.Extensions
{
    public static class IEventBusExtensions
    {
        public static void AddApplicationSubscriptions(this IEventBus eventBus)
        {
            SubscribeToSynchronizationEvents(eventBus);
        }

        private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
        {
            eventBus.Subscribe<DatawalletModifiedIntegrationEvent, DatawalletModifiedIntegrationEventHandler>();
            eventBus.Subscribe<ExternalEventCreatedIntegrationEvent, ExternalEventCreatedIntegrationEventHandler>();
        }
    }
}
