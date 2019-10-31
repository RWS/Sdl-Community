using System.Collections.Generic;
using System.Windows;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	public partial class DeepLWindow
	{
		private static readonly List<string> TargetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES", "PT", "RU"};
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
				LoginTab.ApiKeyBox.Password = credentialStore.Credential;
			}
			if (options != null)
			{
				SettingsTab.Resend.IsChecked = options.ResendDrafts;
			}

			GetSupportedTargetLanguages();
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

			DeepLTabControl.SelectedIndex = 1;
			LoginTab.IsEnabled = false;
		}

		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Options.ApiKey = LoginTab.ApiKeyBox.Password.TrimEnd();
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
		private void GetSupportedTargetLanguages()
		{
			foreach (var languagePair in _languagePairs)
			{
				var targetLanguage = languagePair.TargetCultureName.Substring(0, 2).ToUpper();
				if (TargetSupportedLanguages.Contains(targetLanguage) && !Options.LanguagesSupported.ContainsKey(targetLanguage))
				{
					Options.LanguagesSupported.Add(languagePair.TargetCultureName, "DeepLTranslator");
				}
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
