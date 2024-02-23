using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using LanguageWeaverProvider.Helpers;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for CredentialsMainView.xaml
	/// </summary>
	public partial class CredentialsMainView : Window
	{
		public CredentialsMainView()
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