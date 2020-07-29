using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.Desktop.Platform;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	public partial class DeepLWindow
	{
		private static readonly List<string> TargetSupportedLanguages = new List<string>
		{
			"EN",
			"DE",
			"FR",
			"IT",
			"NL",
			"PL",
			"ES",
			"PT",
			"PT-PT",
			"PT-BR",
			"RU",
		};

		private readonly bool _isTellMeAction;
		public DeepLTranslationOptions Options { get; set; }

		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore = null,
			LanguagePair[] languagePairs = null, bool isTellMeAction = false)
		{
			InitializeComponent();
			_isTellMeAction = isTellMeAction;

			var currentLanguagePairs = isTellMeAction ? options?.LanguagesSupported?.Keys.Select(key => new CultureInfo(key)).ToList() : languagePairs?.Select(lp => new CultureInfo(lp.TargetCultureName)).ToList();

			NotCompatibleBlock.Visibility = Helpers.AreLanguagesCompatibleWithFormalityParameter(currentLanguagePairs)
				? Visibility.Collapsed
				: Visibility.Visible;

			Formality.SelectedIndex = (int)options.Formality;
			PlainText.IsChecked = options.SendPlainText;
			Options = options;

			if (isTellMeAction)
			{
				ApiKeyBox.IsEnabled = false;
			}
			else
			{
				if (credentialStore != null)
				{
					ApiKeyBox.Password = credentialStore.Credential;
				}

				GetSupportedTargetLanguages(languagePairs);
			}
		}

		public DeepLWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Enum.TryParse<Formality>(Formality.SelectedIndex.ToString(), out var formality);
			Options.Formality = formality;
			if (PlainText.IsChecked != null)
			{
				Options.SendPlainText = (bool)PlainText.IsChecked;
			}

			if (_isTellMeAction)
			{
				var editorController = SdlTradosStudio.Application.GetController<EditorController>();
				var documentsOpened = editorController.GetDocuments().Any();

				if (documentsOpened)
				{
					MessageBox.Show(PluginResources.SettingsUpdated_ReopenFilesForEditing,
						PluginResources.SettingsUpdated, MessageBoxButton.OK, MessageBoxImage.Information);
				}
				DialogResult = true;
				Close();
			}
			else
			{
				Options.ApiKey = ApiKeyBox.Password.Trim();
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
		}

		private void GetSupportedTargetLanguages(LanguagePair[] languagePairs)
		{
			foreach (var languagePair in languagePairs)
			{
				var targetLanguage = languagePair.TargetCulture.TwoLetterISOLanguageName.ToUpper();
				if (Helpers.IsSupportedLanguagePair(languagePair.SourceCulture.TwoLetterISOLanguageName.ToUpper(), languagePair.TargetCulture.TwoLetterISOLanguageName.ToUpper()) && !Options.LanguagesSupported.ContainsKey(targetLanguage))
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
	}
}
