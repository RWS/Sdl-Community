using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageWeaverProvider.Helpers;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for PairMappingView.xaml
	/// </summary>
	public partial class PairMappingView : Window
	{
		bool _selectionChanged;

		public PairMappingView()
		{
			InitializeComponent();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AnimationsHelper.StartOpeningWindowAnimation(this);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Closing -= Window_Closing;
			e.Cancel = true;
			AnimationsHelper.StartClosingWindowAnimation(this);
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox.SelectedItem is PairDictionary selectedDictionary
			 && !string.IsNullOrEmpty(selectedDictionary.DictionaryId))
			{
				selectedDictionary.IsSelected = !selectedDictionary.IsSelected;
				_selectionChanged = true;
			}

			comboBox.SelectedItem = null;
		}

		private void ComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (!_selectionChanged)
			{
				return;
			}

			var comboBox = sender as ComboBox;
			comboBox.IsDropDownOpen = true;
			comboBox.SelectedItem = null;
			_selectionChanged = false;
		}
	}
}