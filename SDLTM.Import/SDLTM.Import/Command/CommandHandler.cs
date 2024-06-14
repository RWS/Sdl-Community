using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SDLTM.Import.Command
{
	public class CommandHandler : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;

		public CommandHandler(Action<object> execute)
			: this(execute, null)
		{
		}
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public CommandHandler(Action<object> execute, Predicate<object> canExecute)
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
}
