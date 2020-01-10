using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	public partial class DeepLWindow
	{
		private static readonly List<string> TargetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES", "PT", "RU" };
		private readonly bool _tellMeAction;
		private readonly LanguagePair[] _languagePairs;
		public DeepLTranslationOptions Options { get; set; }

		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			_languagePairs = languagePairs;
			InitializeComponent();
			Options = options;
			if (credentialStore != null)
			{
				ApiKeyBox.Password = credentialStore.Credential;
			}
			PlainText.IsChecked = Options.SendPlainText;

			GetSupportedTargetLanguages();
		}

		public DeepLWindow(DeepLTranslationOptions options, bool tellMeAction)
		{
			InitializeComponent();
			_tellMeAction = tellMeAction;
			Options = options;
			IsEnabled = false;
		}

		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Options.ApiKey = ApiKeyBox.Password.TrimEnd();
			if (PlainText.IsChecked != null)
			{
				Options.SendPlainText = (bool)PlainText.IsChecked;
			}

			if (_tellMeAction)
			{
				DialogResult = true;
				Close();
			}
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
