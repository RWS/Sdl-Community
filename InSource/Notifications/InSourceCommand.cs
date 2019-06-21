using System;
using System.Drawing;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.InSource.Notifications
{
	public class InSourceCommand : IStudioNotificationCommand
	{
		private readonly Action action;
		public event EventHandler CanExecuteChanged;

		public InSourceCommand(Action action)
		{
			this.action = action;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			action();
		}

		public string CommandText { get; set; }
		public string CommandToolTip { get; set; }
		public Icon CommandIcon { get; set; }
	}
}
