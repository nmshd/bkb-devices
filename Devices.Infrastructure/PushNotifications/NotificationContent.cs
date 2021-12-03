﻿using System.Text.Json;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Newtonsoft.Json;
using Enmeshed.Tooling;
using Enmeshed.Tooling.Extensions;

namespace Devices.Infrastructure.PushNotifications
{
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

            SentAt = SystemTime.UtcNow.ToUniversalString();
            AccountReference = recipient;
        }

        public string AccountReference { get; }
        public string EventName { get; }
        public string SentAt { get; }
        public object Payload { get; }
    }
}
