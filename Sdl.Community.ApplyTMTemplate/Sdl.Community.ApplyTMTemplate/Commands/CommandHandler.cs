using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.ApplyTMTemplate.Commands
{
	public class CommandHandler : ICommand, INotifyPropertyChanged
	{
		public event EventHandler CanExecuteChanged;
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

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public void Execute(object parameter)
		{
			_action();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
