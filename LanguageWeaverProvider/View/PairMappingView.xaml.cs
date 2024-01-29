using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
			if (GetWindow(this) is Window window)
			{
				window.DragMove();
			}
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

		private void ComboBox_DropDownClosed(object sender, System.EventArgs e)
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