using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicrosoftTranslatorProvider.Commands
{
	public class AsyncRelayCommand : ICommand
	{
		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;
		private Task _execution;

		public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

		public void Execute(object parameter)
		{
			_execution = _execute.Invoke();
			_ = _execution.ContinueWith(_ => OnCanExecuteChanged());
		}

		protected virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}