﻿using System.Windows;
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

			//Focusing cannot be done very easily through XAML without it looking like an ugly workaround
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
		public void AddNewTermButton_Click(object sender, RoutedEventArgs e)
		{
			((TermbaseViewerViewModel)DataContext).AddNewTermCommand.Execute(sender is TermModel ? sender : null);
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
		/// Here, we are invoking a command from code-behind for two reasons: firstly, the same reason mentioned above
		/// but in regards to another command and secondly, because multiple keyboard modifiers
		/// cannot be specified in code-behind, so PreviewKeyDown cannot be used in all cases.
		/// In order to not use PreviewKeyDown event for some of the keys and KeyBindings for others, we'll just use KeyBindings.
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