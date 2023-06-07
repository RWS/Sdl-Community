using System;
using System.Collections;
using System.Windows.Input;
using Sdl.MultiSelectComboBox.EventArgs;

namespace InterpretBank.Commands
{
	public class SelectedItemsChangedCommand : ICommand
	{
		private readonly Action<ICollection> _updateSelectedItems;

		public SelectedItemsChangedCommand(Action<ICollection> updateSelectedItems)
		{
			_updateSelectedItems = updateSelectedItems;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs args)
			{
				_updateSelectedItems?.Invoke(args.Selected);
			}
		}
	}
}