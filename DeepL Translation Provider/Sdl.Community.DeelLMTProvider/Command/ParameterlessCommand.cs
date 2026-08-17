using System;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.Command
{
	public class ParameterlessCommand : ICommand
	{
		private readonly Func<bool> _canExecute;
		private readonly Action _execute;

		public ParameterlessCommand(Action execute)
			: this(execute, null)
		{
		}

		public ParameterlessCommand(Action execute, Func<bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}
	}
}