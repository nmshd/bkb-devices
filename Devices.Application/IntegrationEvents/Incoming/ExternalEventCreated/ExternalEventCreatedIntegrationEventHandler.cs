using System.Threading.Tasks;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Devices.Application.Infrastructure.PushNotifications;
using Devices.Application.Infrastructure.PushNotifications.ExternalEvents;

namespace Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated
{
    public class ExternalEventCreatedIntegrationEventHandler : IIntegrationEventHandler<ExternalEventCreatedIntegrationEvent>
    {
        private readonly IPushService _pushService;

        public ExternalEventCreatedIntegrationEventHandler(IPushService pushService)
        {
            _pushService = pushService;
        }

        public async Task Handle(ExternalEventCreatedIntegrationEvent @event)
        {
            await _pushService.SendNotificationAsync(@event.Owner, new ExternalEventCreatedPushNotification());
        }
    }
}
