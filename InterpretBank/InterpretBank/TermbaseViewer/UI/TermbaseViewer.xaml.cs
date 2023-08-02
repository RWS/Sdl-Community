using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.TermbaseViewer.Model;

namespace InterpretBank.TermbaseViewer.UI
{
	public partial class TermbaseViewer : UserControl
	{
		public TermbaseViewer()
		{
			InitializeComponent();
		}

		public ICommand SetEditingFromKeyboardCommand => new RelayCommand(SetEditingFromKeyboard);

		private void SetEditingFromKeyboard(object parameter)
		{
			if (!bool.TryParse(parameter.ToString(), out var editing))
			{
				editing = false;
				((TermModel)Term_ListBox.SelectedItem).RevertCommand.Execute(null);
			}
			
			SetEditing(editing);
		}

		public void SetEditing(bool editing)
		{
			((TermModel)Term_ListBox.SelectedItem).IsEditing = editing;
			if (editing)
				SourceTerm_EditableTextBlock.EditBox.Focus();
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			SetEditing(true);
		}

		private void SaveEdit_Button_OnClick(object sender, RoutedEventArgs e)
		{
			SetEditing(false);
		}
	}
}