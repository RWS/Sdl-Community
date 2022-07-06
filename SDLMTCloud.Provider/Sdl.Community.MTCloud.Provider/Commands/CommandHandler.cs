using System;
using System.Windows.Input;

namespace Sdl.Community.MTCloud.Provider.Commands
{
	public class CommandHandler : ICommand
	{
		private readonly Predicate<object> _canExecute;
		private readonly Action<object> _execute;

		public CommandHandler(Action<object> execute) : this(execute, null)
		{
		}

		public CommandHandler(Action<object> execute, Predicate<object> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}