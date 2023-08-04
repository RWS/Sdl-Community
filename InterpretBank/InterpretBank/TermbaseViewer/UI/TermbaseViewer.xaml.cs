using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.TermbaseViewer.Model;
using InterpretBank.TermbaseViewer.ViewModel;

namespace InterpretBank.TermbaseViewer.UI
{
	public partial class TermbaseViewer : UserControl
	{
		public TermbaseViewer()
		{
			InitializeComponent();
		}

		public ICommand SetEditingFromKeyboardCommand => new RelayCommand(SetEditingFromKeyboard);

		public void SetEditing(bool editing)
		{
			((TermModel)Term_ListBox.SelectedItem).IsEditing = editing;
			if (editing)
				SourceTerm_EditableTextBlock.EditBox.Focus();
		}

		/// <summary>
		/// In this section, we are invoking the command directly from code-behind instead of using
		/// command binding for a specific reason. Due to the timing requirements between the command
		/// execution and subsequent UI actions, using command binding would not guarantee the correct
		/// order of operations in this particular scenario.
		///
		/// The invoked command (TermbaseViewerViewModel.AddNewTermCommand) adds a new item to a ListBox,
		/// and the UI event handler needs to focus an EditableTextBlock bound to properties of the newly added item.
		/// This requires the command
		/// to be executed before focusing the EditableTextBlock.
		/// </summary>
		private void AddNewTermButton_Click(object sender, RoutedEventArgs e)
		{
			((TermbaseViewerViewModel)DataContext).AddNewTermCommand.Execute(null);
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

		/// <summary>
		/// In this section, we are invoking the command directly from code-behind instead of using
		/// command binding for a specific reason. Due to the timing requirements between the command
		/// execution and subsequent UI actions, using command binding would not guarantee the correct
		/// order of operations in this particular scenario.
		///
		/// The invoked command (TermbaseViewerViewModel.AddNewTermCommand) adds a new item to a ListBox,
		/// and the UI event handler needs to focus an EditableTextBlock bound to properties of the newly added item.
		/// This requires the command
		/// to be executed before focusing the EditableTextBlock.
		/// </summary>
		private void SetEditingFromKeyboard(object parameter)
		{
			if (!bool.TryParse(parameter.ToString(), out var editing))
			{
				editing = false;
				((TermbaseViewerViewModel)DataContext).RevertCommand.Execute(null);
			}

			SetEditing(editing);
		}
	}
}