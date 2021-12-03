using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Devices.Application.Devices.DTOs;
using MediatR;

namespace Devices.Application.Devices.Commands.DeleteDevice
{
    public class DeleteDeviceCommand : IRequest<Unit>
    {
        public DeviceId DeviceId { get; set; }
        public byte[] DeletionCertificate { get; set; }
        public SignedChallengeDTO SignedChallenge { get; set; }
    }
}
