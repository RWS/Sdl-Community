using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Trados.Transcreate.Commands
{
	public class SelectedItemChangedCommand : ICommand
	{
		private readonly Func<TreeViewItem, bool> _canExecute;
		private readonly Action<TreeViewItem> _execute;

		public SelectedItemChangedCommand(Action<TreeViewItem> execute, Func<TreeViewItem, bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			if (parameter is RoutedPropertyChangedEventArgs<object> args && args.NewValue is TreeViewItem item)
			{
				return _canExecute(item);
			}

			return false;
		}

		public void Execute(object parameter)
		{
			if (parameter is RoutedPropertyChangedEventArgs<object> args && args.NewValue is TreeViewItem item)
			{
				_execute(item);
				return;
			}

			_execute(null);
		}

		public event EventHandler CanExecuteChanged;
	}
}
