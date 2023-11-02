using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
			if (Window.GetWindow(this) is not Window window)
			{
				return;
			}

			window.DragMove();
		}

		private void ComboBox_DropDownClosed(object sender, System.EventArgs e)
		{
			if (_selectionChanged)
			{
				var comboBox = (ComboBox)sender;
				comboBox.IsDropDownOpen = true;
				_selectionChanged = false;
			}
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_selectionChanged = true;
		}
	}
}