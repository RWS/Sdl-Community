using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.CommunityWpfHelpers.Interfaces;

namespace Sdl.CommunityWpfHelpers.Commands
{
	/// <summary>
	/// Awaitable command handler
	/// </summary>
	public class AwaitableCommand: AwaitableCommand<object>,IAsyncCommand
	{
		public AwaitableCommand(Func<Task> executeMethod)
			: base(o => executeMethod())
		{
		}

		public AwaitableCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
			: base(o => executeMethod(), o => canExecuteMethod())
		{
		}
	}
	public class AwaitableCommand<T> : IAsyncCommand<T>, ICommand
	{
		private readonly Func<T, Task> _executeMethod;
		private readonly DelegateCommand<T> _underlyingCommand;
		private bool _isExecuting;

		public AwaitableCommand(Func<T, Task> executeMethod)
			: this(executeMethod, _ => true)
		{
		}

		public AwaitableCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
		{
			_executeMethod = executeMethod;
			_underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
		}

		public async Task ExecuteAsync(T obj)
		{
			try
			{
				_isExecuting = true;
				RaiseCanExecuteChanged();
				await _executeMethod(obj);
			}
			finally
			{
				_isExecuting = false;
				RaiseCanExecuteChanged();
			}
		}

		public ICommand Command => this;

		public bool CanExecute(object parameter)
		{
			return !_isExecuting && _underlyingCommand.CanExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add => _underlyingCommand.CanExecuteChanged += value;
			remove => _underlyingCommand.CanExecuteChanged -= value;
		}

		public async void Execute(object parameter)
		{
			await ExecuteAsync((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			_underlyingCommand.RaiseCanExecuteChanged();
		}
	}
}
