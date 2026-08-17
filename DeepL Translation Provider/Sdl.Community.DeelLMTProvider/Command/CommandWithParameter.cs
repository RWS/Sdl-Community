using System;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.Command
{
	public class CommandWithParameter : ICommand
	{
		private readonly Func<bool> _canExecute;
		private readonly Action<object> _execute;

		public CommandWithParameter(Action<object> execute)
			: this(execute, null)
		{
		}

		public CommandWithParameter(Action<object> execute, Func<bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}