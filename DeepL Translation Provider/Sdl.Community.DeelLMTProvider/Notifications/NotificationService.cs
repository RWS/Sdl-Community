using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Notifications.Views;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Notifications
{
    public static class NotificationService
    {
        public static string NotificationGroupKey = $"{PluginResources.Plugin_Name} pre-translate task errors";

        private static NotificationGroup NotificationGroup { get; } = new()
        {
            Key = NotificationGroupKey,
            Title = NotificationGroupKey,
            Notifications = [],
            IsActionVisible = true,
            Action = new NotificationCommand(ClearNotificationGroupAction)
            {
                CommandToolTip = "Dismiss DeepL error messages",
                CommandIcon = PluginResources.dismiss_icon
            }
        };

        private static IStudioEventAggregator StudioEventAggregator
        {
            get
            {
                try
                {
                    return SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
                }
                catch { return null; }
            }
        }

        public static void Show(List<ErrorItem> errorMessages)
        {
            var newGuid = Guid.NewGuid();
            var firstError = errorMessages.FirstOrDefault();
            var singleError = errorMessages.Count == 1;
            var title = singleError ? "Error" : "One or more errors occured";
            var notification = new Notification
            {
                Id = newGuid,
                AlwaysVisibleDetails = [$"Segment {firstError?.Id}: {firstError?.Message}"],
                Title = title,
                AllowsUserToDismiss = true,
                IsLinkVisible = !singleError,
            };
            notification.ClearNotificationAction = new NotificationCommand(() => ClearNotificationAction(notification));
            notification.LinkAction = new NotificationCommand(() => ShowAllErrors(errorMessages, notification))
            {
                CommandText = "Show all errors",
                CommandToolTip = "Show all errors",
            };

            NotificationGroup.Notifications.Add(notification);

            AddStudioGroupNotificationEvent groupEvent = new(NotificationGroup);
            StudioEventAggregator?.Publish(groupEvent);
        }

        private static void ClearNotificationAction(Notification notififcation)
        {
            NotificationGroup.Notifications.Remove(notififcation);
            StudioEventAggregator.Publish(
                new RemoveStudioNotificationFromGroupEvent(NotificationGroupKey, notififcation.Id));
        }

        private static void ClearNotificationGroupAction()
        {
            NotificationGroup.Notifications.Clear();
            StudioEventAggregator.Publish(new RemoveStudioGroupNotificationEvent(NotificationGroupKey));
        }

        private static void ShowAllErrors(List<ErrorItem> errorMessages, Notification notification)
        {
            //ClearNotificationAction(notification);
            var messageService = new ErrorsWindow(errorMessages);
            messageService.ShowDialog();
        }
    }
}