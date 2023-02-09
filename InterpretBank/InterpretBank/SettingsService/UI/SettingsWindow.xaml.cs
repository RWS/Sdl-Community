using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterpretBank.SettingsService.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;

			MoveTextToValueHolder(sender);
		}

		//This is used to hold the value as we clear the textbox
		//We need this value to send as a parameter for adding a new db value
		//Otherwise we'd have to create ChangeNotification properties for each textbox, which is less clean
		private void MoveTextToValueHolder(object sender)
		{
			ValueHolder.Text = (sender as TextBox).Text;
			(sender as TextBox).Clear();
		}
	}
}
