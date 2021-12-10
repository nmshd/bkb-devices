using System.Dynamic;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devices.Infrastructure.PushNotifications
{
    /// <summary>
    ///     Final format:
    ///     {
    ///     "data": {
    ///     "android_channel_id": "ENMESHED",
    ///     "title": string,
    ///     "notId": number,
    ///     "accRef": string,
    ///     "eventName": string,
    ///     "sentAt": string,
    ///     "payload": any
    ///     }
    ///     }
    /// </summary>
    public class FcmNotificationBuilder : NotificationBuilder
    {
        private readonly dynamic _notification;

        private FcmNotificationBuilder()
        {
            _notification = new ExpandoObject();
            _notification.data = new ExpandoObject();
            _notification.data.content = new ExpandoObject();
        }

        public static FcmNotificationBuilder BuildDefaultNotification()
        {
            var builder = new FcmNotificationBuilder();
            builder.SetAndroidChannelId("ENMESHED");
            return builder;
        }

        private NotificationBuilder SetAndroidChannelId(string channelId)
        {
            _notification.data.android_channel_id = channelId;
            return this;
        }

        public override NotificationBuilder AddContent(NotificationContent content)
        {
            _notification.data.content.accRef = content.AccountReference;
            _notification.data.content.eventName = content.EventName;
            _notification.data.content.sentAt = content.SentAt;
            _notification.data.content.payload = content.Payload;
            return this;
        }

        public override NotificationBuilder SetNotificationText(string text)
        {
            _notification.data.title = text;
            return this;
        }

        public override NotificationBuilder SetContentAvailable(string contentAvailable)
        {
            ((IDictionary<string, object>) _notification.data)["content-available"] = contentAvailable;
            return this;
        }

        public override NotificationBuilder SetNotificationId(int notificationId)
        {
            _notification.data.notId = notificationId;
            return this;
        }

        public override Notification Create()
        {
            var payload = JsonConvert.SerializeObject(_notification, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            var notification = new FcmNotification(payload);
            return notification;
        }
    }
}
