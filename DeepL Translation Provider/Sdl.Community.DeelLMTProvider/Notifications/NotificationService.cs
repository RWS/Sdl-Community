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

        public static List<ErrorItem> ErrorMessages { get; set; } = [];

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
            ErrorMessages.AddRange(errorMessages);
            var newGuid = Guid.NewGuid();
            var firstError = errorMessages.LastOrDefault();
            var singleError = ErrorMessages.Count == 1;
            var title = singleError ? "Error" : "One or more errors occured";
            var notification = new Notification
            {
                Id = newGuid,
                AlwaysVisibleDetails = [$"Segment {firstError?.Id}: {firstError?.Message}"],
                Title = title,
                AllowsUserToDismiss = true,
                IsLinkVisible = true,
            };
            notification.ClearNotificationAction = new NotificationCommand(() => ClearNotificationAction(notification))
            {
                CommandToolTip = "Dismiss DeepL error messages",
            };
            var linkAction = new NotificationCommand(
                singleError
                    ? () => ShowErrorDetails(errorMessages.FirstOrDefault())
                    : () => ShowAllErrors(ErrorMessages))
            {
                CommandText = "Show more",
                CommandToolTip = "Show more",
            };

            notification.LinkAction = linkAction;

            NotificationGroup.Notifications = [notification];

            AddStudioGroupNotificationEvent groupEvent = new(NotificationGroup);
            StudioEventAggregator?.Publish(groupEvent);
        }

        private static void ClearNotificationAction(Notification notififcation)
        {
            ErrorMessages.Clear();
            NotificationGroup.Notifications.Remove(notififcation);
            StudioEventAggregator.Publish(
                new RemoveStudioNotificationFromGroupEvent(NotificationGroupKey, notififcation.Id));
        }

        private static void ClearNotificationGroupAction()
        {
            ErrorMessages.Clear();
            NotificationGroup.Notifications.Clear();
            StudioEventAggregator.Publish(new RemoveStudioGroupNotificationEvent(NotificationGroupKey));
        }

        private static void ShowAllErrors(List<ErrorItem> errorMessages)
        {
            var sanitizedErrorMessages = errorMessages?.Select(error => new ErrorItem
            {
                Id = error.Id,
                Message = error.Message ?? "N/A"
            }).ToList() ?? [];

            var messageService = new ErrorsWindow(sanitizedErrorMessages);
            messageService.ShowDialog();
        }

        private static void ShowErrorDetails(ErrorItem error) => ErrorsWindow.ShowError(error);
    }
}