using AutoMapper;
using Devices.Application.Infrastructure.PushNotifications;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration
{
    public class Handler : IRequestHandler<UpdateDeviceRegistrationCommand, Unit>
    {
        private readonly IdentityAddress _activeIdentity;
        private readonly IMapper _mapper;
        private readonly IPushService _pushService;

        public Handler(IPushService pushService, IUserContext userContext, IMapper mapper)
        {
            _pushService = pushService;
            _mapper = mapper;
            _activeIdentity = userContext.GetAddress();
        }

        public async Task<Unit> Handle(UpdateDeviceRegistrationCommand request, CancellationToken cancellationToken)
        {
            var deviceRegistration = new DeviceRegistration(request.Platform, request.Handle, request.InstallationId);
            await _pushService.RegisterDeviceAsync(_activeIdentity, deviceRegistration);
            return Unit.Value;
        }
    }
}
