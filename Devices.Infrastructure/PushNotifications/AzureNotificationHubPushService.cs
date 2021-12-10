using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Devices.Application.Infrastructure.PushNotifications;
using Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace Devices.Infrastructure.PushNotifications;

public class AzureNotificationHubPushService : IPushService
{
    private static readonly NotificationPlatform[] SupportedPlatforms = {NotificationPlatform.Fcm, NotificationPlatform.Apns};
    private readonly ILogger<AzureNotificationHubPushService> _logger;
    private readonly NotificationHubClient _notificationHubClient;

    public AzureNotificationHubPushService(NotificationHubClient notificationHubClient, ILogger<AzureNotificationHubPushService> logger)
    {
        _notificationHubClient = notificationHubClient;
        _logger = logger;
    }

    public async Task RegisterDeviceAsync(IdentityAddress identityId, DeviceRegistration registration)
    {
        var installation = new Installation
        {
            InstallationId = registration.InstallationId.ToString(),
            PushChannel = registration.Handle,
            Tags = GetNotificationTags(identityId),
            Platform = registration.Platform switch
            {
                "apns" => NotificationPlatform.Apns,
                "fcm" => NotificationPlatform.Fcm,
                _ => throw new ArgumentException($"There is no corresponding platform for {registration.Platform}.", nameof(registration.Platform))
            }
        };

        await _notificationHubClient.CreateOrUpdateInstallationAsync(installation);

        _logger.LogTrace("New device successfully registered.");
    }

    public async Task SendNotificationAsync(IdentityAddress recipient, object pushNotification)
    {
        var notificationContent = new NotificationContent(recipient, pushNotification);
        var notificationId = GetNotificationId(pushNotification);
        var notificationText = GetNotificationText(pushNotification);

        foreach (var notificationPlatform in SupportedPlatforms)
        {
            var notification = NotificationBuilder
                .BuildDefaultNotification(notificationPlatform)
                .SetNotificationText(notificationText)
                .SetNotificationId(notificationId)
                .AddContent(notificationContent)
                .Create();

            await _notificationHubClient.SendNotificationAsync(notification, GetNotificationTags(recipient));

            _logger.LogTrace($"Successfully sent push notification: {notification.ToJson()}");
        }
    }

    private static string GetNotificationText(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationTextAttribute>();
        return attribute == null ? "" : attribute.Value;
    }

    private static int GetNotificationId(object pushNotification)
    {
        var attribute = pushNotification.GetType().GetCustomAttribute<NotificationIdAttribute>();
        return attribute?.Value ?? 0;
    }

    private static IList<string> GetNotificationTags(IdentityAddress identityId)
    {
        var identityTag = "identityId:" + identityId;
        return new[] {identityTag};
    }
}

public static class TypeExtensions
{
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T) type.GetCustomAttribute(typeof(T));
    }
}

public static class NotificationExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string ToJson(this Notification notification)
    {
        return JsonSerializer.Serialize(notification, SerializerOptions);
    }
}
