using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Sdl.LanguagePlatform.Core;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
		private static readonly List<string> TargetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES", "PT", "RU" };
		private readonly LanguagePair[] _languagePairs;

		public DeepLTranslationOptions Options { get; set; }
		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			InitializeComponent();
			Options = options;
			_languagePairs = languagePairs;
			if (credentialStore != null)
			{
				ApiKeyBox.Password = credentialStore.Credential;
			}
			GetSupportedTargetLanguages();
		}
		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Options.ApiKey = ApiKeyBox.Password.TrimEnd();
			if (!string.IsNullOrEmpty(Options.ApiKey))
			{
				ValidationBlock.Visibility = Visibility.Hidden;
				DialogResult = true;
				Close();
			}
			else
			{
				ValidationBlock.Visibility = Visibility.Visible;
			}
		}
		private void GetSupportedTargetLanguages()
		{
			foreach (var languagePair in _languagePairs)
			{
				var targetLanguage = languagePair.TargetCulture.TwoLetterISOLanguageName.ToUpper();
				if (TargetSupportedLanguages.Contains(targetLanguage) && !Options.LanguagesSupported.ContainsKey(targetLanguage))
				{
					if (!Options.LanguagesSupported.ContainsKey(languagePair.TargetCultureName))
					{
						Options.LanguagesSupported.Add(languagePair.TargetCultureName, "DeepLTranslator");
					}
				}
			}
		}
		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start("https://www.deepl.com/api-contact.html");
		}
		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
