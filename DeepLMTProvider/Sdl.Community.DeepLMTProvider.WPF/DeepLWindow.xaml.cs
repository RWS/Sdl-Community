using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.Community.DeepLMTProvider.WPF.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	/// <summary>
	/// Interaction logic for DeepLWindow.xaml
	/// </summary>
	public partial class DeepLWindow
	{
		private bool _tellMeAction;
		public DeepLTranslationOptions Options { get; set; }
		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore)
		{
			InitializeComponent();
			Options = options;
			if (credentialStore != null)
			{
				LoginTab.ApiKeyBox.Password = credentialStore.Credential;
			}
			if (options != null)
			{
				SettingsTab.Resend.IsChecked = options.ResendDrafts;
			}
		}
		public DeepLWindow(DeepLTranslationOptions options, bool tellMeAction)
		{
			InitializeComponent();
			_tellMeAction = tellMeAction;
			Options = options;
			if (options != null)
			{
				SettingsTab.Resend.IsChecked = options.ResendDrafts;
			}
		}

		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Options.ApiKey = LoginTab.ApiKeyBox.Password;
			if (SettingsTab.Resend.IsChecked != null)
			{
				Options.ResendDrafts = SettingsTab.Resend.IsChecked.Value;
			}
			if (_tellMeAction)
			{
				DialogResult = true;
				Close();
			}
			if (!string.IsNullOrEmpty(Options.ApiKey))
			{
				LoginTab.ValidationBlock.Visibility = Visibility.Hidden;
				DialogResult = true;
				Close();
			}
			else
			{
				LoginTab.ValidationBlock.Visibility = Visibility.Visible;
			}
			
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
