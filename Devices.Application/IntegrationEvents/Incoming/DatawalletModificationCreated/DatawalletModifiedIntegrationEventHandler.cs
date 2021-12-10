using AutoMapper;
using Devices.Application.Infrastructure.PushNotifications;
using Devices.Application.Infrastructure.PushNotifications.Datawallet;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated
{
    public class DatawalletModifiedIntegrationEventHandler : IIntegrationEventHandler<DatawalletModifiedIntegrationEvent>
    {
        private readonly IMapper _mapper;
        private readonly IPushService _pushService;

        public DatawalletModifiedIntegrationEventHandler(IPushService pushService, IMapper mapper)
        {
            _pushService = pushService;
            _mapper = mapper;
        }

        public async Task Handle(DatawalletModifiedIntegrationEvent integrationEvent)
        {
            await _pushService.SendNotificationAsync(integrationEvent.Identity, new DatawalletModificationsCreatedPushNotification(integrationEvent.ModifiedByDevice));
        }
    }
}
