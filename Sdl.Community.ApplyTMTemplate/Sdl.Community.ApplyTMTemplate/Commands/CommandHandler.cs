using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sdl.Community.ApplyTMTemplate.Commands
{
	public class CommandHandler : ICommand, INotifyPropertyChanged
	{
		private readonly Action _action;

		private readonly bool _canExecute;

		public CommandHandler(Action action, bool canExecute)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public CommandHandler(bool canExecute)
		{
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public void Execute(object parameter)
		{
			_action();
		}
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}