using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;

namespace Devices.Infrastructure.PushNotifications
{
    public abstract class NotificationBuilder
    {
        protected readonly Dictionary<string, string> _headers = new();

        public static NotificationBuilder BuildDefaultNotification(NotificationPlatform platform)
        {
            NotificationBuilder builder = platform switch
            {
                NotificationPlatform.Fcm => FcmNotificationBuilder.BuildDefaultNotification(),
                NotificationPlatform.Apns => ApnsNotificationBuilder.BuildDefaultNotification(),
                _ => throw new ArgumentException($"The platform {platform} is not supported.")
            };

            builder.SetContentAvailable("1");

            return builder;
        }

        public abstract NotificationBuilder AddContent(NotificationContent content);

        public abstract NotificationBuilder SetNotificationText(string text);

        public abstract NotificationBuilder SetContentAvailable(string contentAvailable);

        public abstract NotificationBuilder SetNotificationId(int notificationId);

        public NotificationBuilder AddHeader(string name, string value)
        {
            _headers.Add(name, value);
            return this;
        }

        public abstract Notification Create();
    }
}
