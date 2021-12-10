namespace Devices.Application.Infrastructure.PushNotifications;

public class NotificationTextAttribute : Attribute
{
    public NotificationTextAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
