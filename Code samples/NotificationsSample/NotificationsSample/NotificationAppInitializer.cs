using System;
using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace NotificationsSample
{
	[ApplicationInitializer]
	public class NotificationAppInitializer : IApplicationInitializer
	{
		private const string NotificationGroupId = "b0261aa3-b6a5-4f69-8f94-3713784ce8ea";
		private SampleNotificationGroup _notificationGroup;
		private IStudioEventAggregator _eventAggregator;

		public void Execute()
		{
			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			_eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
			_notificationGroup = new SampleNotificationGroup(NotificationGroupId)
			{
				Title = "Sample notifications"
			};
		}

		private void ClearNotification(SampleNotification notification)
		{
			_eventAggregator.Publish(new RemoveStudioNotificationFromGroupEvent(NotificationGroupId, notification.Id));
		}

		private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent e)
		{
			CreateNotification("First notification", "First description");
			CreateNotification("Second notification", "Second description");

			// add group notification
			var groupEvent = new AddStudioGroupNotificationEvent(_notificationGroup);
			_eventAggregator.Publish(groupEvent);
		}

		private void CreateNotification(string title, string description)
		{
			var notification = new SampleNotification(Guid.NewGuid())
			{
				Title = title,
				AlwaysVisibleDetails = new List<string>
				{
					description
				},
				AllowsUserToDismiss = true
			};

			_notificationGroup.Notifications.Add(notification);
			Action action = () => ClearNotification(notification);
			var clearNotificationCommand = new NotificationCommand(action);
			notification.ClearNotificationAction = clearNotificationCommand;
		}
	}
}

