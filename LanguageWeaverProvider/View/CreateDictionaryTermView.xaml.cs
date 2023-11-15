using System.Windows;

namespace LanguageWeaverProvider.View
{
	public partial class CreateDictionaryTermView : Window
	{
		public CreateDictionaryTermView()
		{
			InitializeComponent();
		}

		private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (GetWindow(this) is Window window)
			{
				window.DragMove();
			}
		}
	}
}
