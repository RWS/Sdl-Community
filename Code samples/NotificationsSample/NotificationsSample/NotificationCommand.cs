using System;
using System.Drawing;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace NotificationsSample
{
	public class NotificationCommand : IStudioNotificationCommand
	{
		private readonly Action _action;
		public event EventHandler CanExecuteChanged;

		public NotificationCommand(Action action)
		{
			_action = action;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_action();
		}

		public string CommandText { get; set; }
		public string CommandToolTip { get; set; }
		public Icon CommandIcon { get; set; }
	}
}
