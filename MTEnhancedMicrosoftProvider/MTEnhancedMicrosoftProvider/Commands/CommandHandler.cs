using System;
using System.Windows.Input;

namespace MTEnhancedMicrosoftProvider.Commands
{
	public class CommandHandler : ICommand
	{
		public event EventHandler CanExecuteChanged;
		private readonly Action _action;
		private readonly bool _canExecute;

		public CommandHandler(Action action, bool canExecute)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public CommandHandler(bool canExecute)
		{
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public void Execute(object parameter)
		{
			_action();
		}
	}
}