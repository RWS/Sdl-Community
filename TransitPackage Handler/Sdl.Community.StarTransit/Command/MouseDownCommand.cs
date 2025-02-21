using System;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.StarTransit.Extension;
using Sdl.Community.StarTransit.Interface;

namespace Sdl.Community.StarTransit.Command
{
	public class MouseDownCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly IProgressHeader _progressHeader;

		public MouseDownCommand(IProgressHeader progressHeader)
		{
			_progressHeader = progressHeader;
		}

		public void Execute(object parameter)
		{
			var eventArgs = parameter as MouseButtonEventArgs;
			if (eventArgs?.Source is FrameworkElement frameworkElement)
			{
				if (frameworkElement.DataContext is IProgressHeaderItem progressHeaderItem)
				{
					_progressHeader.MoveToSelectedPage(progressHeaderItem);
				}
			}
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged.Raise(this);
		}
	}
}
