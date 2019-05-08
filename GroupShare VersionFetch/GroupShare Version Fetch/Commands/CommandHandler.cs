using System;

namespace Sdl.Community.GSVersionFetch.Commands
{
	public class CommandHandler
	{
		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;

		public CommandHandler(Action<object> execute) : this(execute, null) { }

		public CommandHandler(Action<object> execute, Predicate<object> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public event EventHandler CanExecuteChanged;
	}
}
