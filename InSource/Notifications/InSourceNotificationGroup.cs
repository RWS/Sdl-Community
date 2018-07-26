using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.InSource.Notifications
{
	public class InSourceNotificationGroup:IStudioGroupNotification
	{
		private readonly ObservableCollection<IStudioNotification> _notifications;
		public InSourceNotificationGroup(string key)
		{
			Key = key;
			_notifications = new ObservableCollection<IStudioNotification>();
		}
		public string Title { get; set; }
		public IStudioNotificationCommand Action { get; set; }
		public bool IsActionVisible { get; set; }
		public string Key { get; }
		public ObservableCollection<IStudioNotification> Notifications
		{
			get => _notifications; set { }
		}
	}
}
