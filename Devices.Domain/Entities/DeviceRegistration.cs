using System;

namespace Devices.Domain.Entities
{
    public class DeviceRegistration
    {
#pragma warning disable CS8618
        private DeviceRegistration() { }
#pragma warning restore CS8618

        public DeviceRegistration(string platform, string handle, Guid installationId)
        {
            Platform = platform;
            Handle = handle;
            InstallationId = installationId;
        }

        public string Platform { get; }
        public string Handle { get; }
        public Guid InstallationId { get; }
    }
}
