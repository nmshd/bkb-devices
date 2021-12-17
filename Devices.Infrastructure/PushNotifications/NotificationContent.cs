using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.Tooling.Extensions;
using Newtonsoft.Json;

namespace Devices.Infrastructure.PushNotifications;

public class NotificationContent
{
    private const string PUSH_NOTIFICATION_POSTFIX = "PushNotification";

    public NotificationContent(IdentityAddress recipient, object pushNotification)
    {
        var notificationTypeName = pushNotification.GetType().Name;

        if (!notificationTypeName.Contains(PUSH_NOTIFICATION_POSTFIX))
        {
            EventName = "dynamic";

            if (pushNotification is JsonElement jsonElement)
                Payload = JsonConvert.DeserializeObject<object>(jsonElement.GetRawText());
            else
                Payload = pushNotification;
        }
        else
        {
            EventName = notificationTypeName.Replace(PUSH_NOTIFICATION_POSTFIX, "");
            Payload = pushNotification;
        }

        SentAt = SystemTime.UtcNow;
        AccountReference = recipient;
    }

    [JsonPropertyName("accRef")]
    public string AccountReference { get; }

    [JsonPropertyName("eventName")]
    public string EventName { get; }

    [JsonPropertyName("sentAt")]
    public DateTime SentAt { get; }

    [JsonPropertyName("payload")]
    public object Payload { get; }
}
