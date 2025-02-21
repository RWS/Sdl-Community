using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.Command
{
	public class AsyncParameterlessCommand : ICommand
	{
		private readonly Func<Task> _execute;
		private bool _isExecuting;

		public AsyncParameterlessCommand(Func<Task> execute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return !_isExecuting;
		}

		public async void Execute(object parameter)
		{
			if (CanExecute(parameter))
			{
				try
				{
					_isExecuting = true;
					RaiseCanExecuteChanged();
					await _execute();
				}
				finally
				{
					_isExecuting = false;
					RaiseCanExecuteChanged();
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}