using System;
using System.Windows.Input;

namespace MTEnhancedMicrosoftProvider.Commands
{
	public class RelayCommand : ICommand
	{
		Action<object> execute;
		Predicate<object> canExecute;


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
			return canExecute is null || canExecute((object)parameter);
		}

		public void Execute(object parameter)
		{
			execute((object)parameter);
		}
	}


	public class RelayCommand<T> : ICommand
	{
		Action<T> execute;
		Predicate<T> canExecute;


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