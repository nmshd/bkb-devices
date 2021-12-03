using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devices.Infrastructure.PushNotifications
{
    /// <summary>
    ///     # Final format:
    ///     ## Headers:
    ///     - apns-priority: 5
    ///     - apns-collapse-id: 0
    ///     ## Payload:
    ///     {
    ///     "data": {
    ///     "accRef":"a",
    ///     "eventName":"dynamic",
    ///     "sentAt":"2020-03-24T14:18:23.906Z",
    ///     "payload":"{'Some':'Payload'}"   },
    ///     "aps":{
    ///     "alert": ""
    ///     }
    ///     }
    ///     }
    /// </summary>
    public class ApnsNotificationBuilder : NotificationBuilder
    {
        private readonly dynamic _notification;

        private ApnsNotificationBuilder()
        {
            _notification = new ExpandoObject();
            _notification.content = new ExpandoObject();
            _notification.aps = new ExpandoObject();
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
            _notification.content.accRef = content.AccountReference;
            _notification.content.eventName = content.EventName;
            _notification.content.sentAt = content.SentAt;
            _notification.content.payload = content.Payload;
            return this;
        }

        public override NotificationBuilder SetNotificationText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                _notification.aps.alert = text;

            return this;
        }

        public override NotificationBuilder SetContentAvailable(string contentAvailable)
        {
            ((IDictionary<string, object>) _notification.aps)["content-available"] = contentAvailable;
            return this;
        }

        public override NotificationBuilder SetNotificationId(int notificationId)
        {
            _notification.notId = notificationId;
            return this;
        }

        public override Notification Create()
        {
            var serializedNotification = JsonConvert.SerializeObject(_notification, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            var appleNotification = new AppleNotification(serializedNotification, _headers);
            return appleNotification;
        }
    }
}
