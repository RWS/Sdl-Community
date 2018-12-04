using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;

		public RelayCommand(Action<object> execute)
			: this(execute, null)
		{
		}
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException("execute");
			_canExecute = canExecute;
		}
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

	}
	public class RelayCommand<T> : ICommand
	{
		private readonly Predicate<T> _canExecute;
		private readonly Action<T> _execute;

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
			_execute = execute;
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException("execute");
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}
