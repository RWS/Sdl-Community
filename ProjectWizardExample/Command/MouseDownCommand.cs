using System;
using System.Windows;
using System.Windows.Input;
using ProjectWizardExample.Extensions;
using ProjectWizardExample.Wizard.API;

namespace ProjectWizardExample.Command
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
			if (parameter != null)
			{
				var eventArgs = parameter as MouseButtonEventArgs;
				if (eventArgs?.Source is FrameworkElement frameworkElement)
				{
					if (frameworkElement.DataContext is IProgressHeaderItem progressHeaderItem)
					{
						_progressHeader.VisitSelectedPage(progressHeaderItem);
					}
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
