using System;
using System.Windows.Input;

namespace Sdl.Community.MTEdge.Provider.Command
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		public RelayCommand(Action<object> action) : this(action, null) { }

		public RelayCommand(Action<object> action, Predicate<object> predicate)
		{
			execute = action;
			canExecute = predicate;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return canExecute is null || canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}
	}

	public class RelayCommand<T> : ICommand
	{
		private readonly Action<T> execute;
		private readonly Predicate<T> canExecute;

		public RelayCommand(Action<T> action) : this(action, null) { }

		public RelayCommand(Action<T> action, Predicate<T> predicate)
		{
			execute = action;
			canExecute = predicate;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return canExecute is null || canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			execute((T)parameter);
		}
	}
}