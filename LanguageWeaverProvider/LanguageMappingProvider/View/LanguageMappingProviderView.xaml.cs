using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LanguageWeaverProvider.Helpers;

namespace LanguageWeaverProvider.LanguageMappingProvider.View
{
	/// <summary>
	/// Interaction logic for LanguageMappingProviderView.xaml
	/// </summary>
	public partial class LanguageMappingProviderView : Window
    {
        public LanguageMappingProviderView()
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