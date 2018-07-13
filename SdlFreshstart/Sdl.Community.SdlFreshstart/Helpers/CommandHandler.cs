using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.Helpers
{
    public class CommandHandler : ICommand, INotifyPropertyChanged
	{
		private Action _action;
		private bool _canExecute;
		public CommandHandler(Action action, bool canExecute)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_action();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
