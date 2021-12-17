using System;
using System.Collections.Generic;
using System.Text.Json;
using Devices.Infrastructure.PushNotifications;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Xunit;

namespace Devices.Infrastructure.Tests.Tests
{
    public class ApnsNotificationBuilderTests
    {
        [Fact]
        public void Test1()
        {
            SystemTime.Set(DateTime.Parse("2021-01-01T00:00:00.000Z"));

            var builtNotification = NotificationBuilder
                .Create(NotificationPlatform.Apns)
                .SetNotificationId(1)
                .SetNotificationText("someNotificationTextTitle", "someNotificationTextBody")
                .AddContent(new NotificationContent(IdentityAddress.Create(new byte[] {0}, "id1"), new {SomeProperty = "someValue"}))
                .Build();

            var formattedBuiltNotification = FormatJson(builtNotification.Body);

            var expectedNotification = FormatJson(@"{
                'notId': 1,
                'content': {
                    'accRef': 'id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j',
                    'eventName': 'dynamic',
                    'sentAt': '2021-01-01T00:00:00.000Z',
                    'payload': {
                        'someProperty': 'someValue'
                    }
                },
                'aps': {
                    'content-available': '1',
                    'alert': {
                        'title': 'someNotificationTextTitle',
                        'body': 'someNotificationTextBody'
                    }
                }
            }");

            formattedBuiltNotification.Should().Be(expectedNotification);

            builtNotification.Headers.Should().HaveCount(1);
            builtNotification.Headers.Should().Contain(new KeyValuePair<string, string>("apns-priority", "5"));
        }

        private string FormatJson(string jsonString)
        {
            jsonString = jsonString.Replace("'", "\"");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
            return JsonSerializer.Serialize(jsonElement, options);
        }
    }
}
