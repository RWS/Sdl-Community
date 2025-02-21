using System.ComponentModel;
using System.Windows;
using LanguageWeaverProvider.Helpers;
using System.Windows.Input;

namespace LanguageWeaverProvider.Studio.Actions.View
{
	public partial class CreateDictionaryTermView : Window
	{
		public CreateDictionaryTermView()
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
	}
}