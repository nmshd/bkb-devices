using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.NotificationHubs;

namespace Devices.Infrastructure.PushNotifications;

/// <summary>
///     See corresponding Unit Tests for an example of a built notification.
/// </summary>
public class ApnsNotificationBuilder : NotificationBuilder
{
    private readonly Payload _notification = new();

    private ApnsNotificationBuilder()
    {
    }

    public static ApnsNotificationBuilder BuildDefaultNotification()
    {
        var builder = new ApnsNotificationBuilder();

        builder
            .AddHeader("apns-priority", "5");

        return builder;
    }

    public override NotificationBuilder AddContent(NotificationContent content)
    {
        _notification.Content = content;
        // _notification.Content.AccountReference = content.AccountReference;
        // _notification.Content.EventName = content.EventName;
        // _notification.Content.SentAt = content.SentAt;
        // _notification.Content.Payload = content.Payload;

        SetContentAvailable(true);

        return this;
    }

    public override NotificationBuilder SetNotificationText(string title, string body)
    {
        if (!string.IsNullOrWhiteSpace(title))
            _notification.APS.Alert.Title = title;

        if (!string.IsNullOrWhiteSpace(body))
            _notification.APS.Alert.Body = body;

        return this;
    }
    
    private void SetContentAvailable(bool contentAvailable)
    {
        _notification.APS.ContentAvailable = contentAvailable ? "1" : "0";
    }

    public override NotificationBuilder SetNotificationId(int notificationId)
    {
        _notification.NotificationId = notificationId;
        return this;
    }

    public override Notification Create()
    {
        var serializedPayload = JsonSerializer.Serialize(_notification, _jsonSerializerOptions);
        var notification = new AppleNotification(serializedPayload, _headers);
        return notification;
    }

    private class Payload
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        [JsonPropertyName("notId")]
        public int NotificationId { get; set; }

        [JsonPropertyName("content")]
        public NotificationContent Content { get; set; }

        [JsonPropertyName("aps")]
        public PayloadAps APS { get; } = new();

        public class PayloadContent
        {

            [JsonPropertyName("accRef")]
            public string AccountReference { get; set; }

            [JsonPropertyName("eventName")]
            public string EventName { get; set; }

            [JsonPropertyName("sentAt")]
            public DateTime SentAt { get; set; }

            [JsonPropertyName("payload")]
            public object Payload { get; set; }
        }

        public class PayloadAps
        {
            [JsonPropertyName("content-available")]
            public string ContentAvailable { get; set; }

            [JsonPropertyName("alert")]
            public ApsAlert Alert { get; } = new();

            public class ApsAlert
            {
                [JsonPropertyName("title")]
                public string Title { get; set; }

                [JsonPropertyName("body")]
                public string Body { get; set; }
            }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
