using System.Threading.Tasks;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Devices.Domain.Entities;

namespace Devices.Application.Infrastructure.PushNotifications
{
    public interface IPushService
    {
        Task SendNotificationAsync(IdentityAddress recipient, object notification);
        Task RegisterDeviceAsync(IdentityAddress identityId, DeviceRegistration registration);
    }
}
