using System;
using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.InSource.Notifications
{
	public class InSourceNotification: IStudioNotification
	{
		public InSourceNotification(Guid id)
		{
			Id = id;
		}
		public Guid Id { get; set; }
		public string Title { get; set; }
		public IStudioNotificationCommand LinkAction { get; set; }
		public bool IsExpanderVisible { get; set; }
		public bool IsLinkVisible { get; set; }
		public List<string> AlwaysVisibleDetails { get; set; }
		public List<string> OtherDetails { get; set; }
		public IStudioNotificationCommand Action { get; set; }
		public bool IsActionVisible { get; set; }
		public bool AllowsUserToDismiss { get; set; }
		public IStudioNotificationCommand ClearNotificationAction { get; set; }
	}
}
