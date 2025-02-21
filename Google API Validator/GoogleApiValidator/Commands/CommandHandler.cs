using System;
using System.Windows.Input;
using Sdl.Community.GoogleApiValidator.Model;

namespace Sdl.Community.GoogleApiValidator.Commands
{
	public class CommandHandler : ModelBase, ICommand
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
