using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Helpers;

namespace MicrosoftTranslatorProvider.View
{
	/// <summary>
	/// Interaction logic for ProviderConfigurationView.xaml
	/// </summary>
	public partial class ProviderConfigurationView : Window
    {
        public ProviderConfigurationView()
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