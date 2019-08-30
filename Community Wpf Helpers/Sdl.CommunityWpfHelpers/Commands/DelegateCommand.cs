using System;
using System.Windows.Input;
using Sdl.CommunityWpfHelpers.Interfaces;

namespace Sdl.CommunityWpfHelpers.Commands
{
	/// <summary>
	/// USED FOR AWAITABLE COMMAND
	/// </summary>
	public class DelegateCommand : DelegateCommand<object>
	{
		public DelegateCommand(Action executeMethod)
			: base(o => executeMethod())
		{
		}

		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
			: base(o => executeMethod(), o => canExecuteMethod())
		{
		}
	}

	/// <summary>
	/// A command that calls the specified delegate when the command is executed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
	{
		private readonly Func<T, bool> _canExecuteMethod;
		private readonly Action<T> _executeMethod;
		private bool _isExecuting;

		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null)
		{
		}

		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
		{
			if ((executeMethod == null) && (canExecuteMethod == null))
			{
				throw new ArgumentNullException(nameof(executeMethod), @"Execute Method cannot be null");
			}
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		bool ICommand.CanExecute(object parameter)
		{
			return !_isExecuting && CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			_isExecuting = true;
			try
			{
				RaiseCanExecuteChanged();
				Execute((T)parameter);
			}
			finally
			{
				_isExecuting = false;
				RaiseCanExecuteChanged();
			}
		}

		public bool CanExecute(T parameter)
		{
			if (_canExecuteMethod == null)
				return true;

			return _canExecuteMethod(parameter);
		}

		public void Execute(T parameter)
		{
			_executeMethod(parameter);
		}
	}
}
