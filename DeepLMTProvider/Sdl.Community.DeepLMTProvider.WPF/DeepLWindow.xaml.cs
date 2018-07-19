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
		public DeepLTranslationOptions Options { get; set; }
		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore)
		{
			InitializeComponent();
			Options = options;
			if (credentialStore != null)
			{
				LoginTab.ApiKeyBox.Password = credentialStore.Credential;
			}
		}
		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Options.ApiKey = LoginTab.ApiKeyBox.Password;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
