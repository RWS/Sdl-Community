using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	public partial class DeepLWindow
	{
		private readonly bool _isTellMeAction;

		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore = null,
			LanguagePair[] languagePairs = null, bool isTellMeAction = false)
		{
			InitializeComponent();
			_isTellMeAction = isTellMeAction;

			var currentLanguagePairs = isTellMeAction
				? options?.LanguagesSupported?.Keys.Select(key => new CultureInfo(key)).ToList()
				: languagePairs?.Select(lp => new CultureInfo(lp.TargetCultureName)).ToList();

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
			}
		}

		public DeepLTranslationOptions Options { get; set; }

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start("https://www.deepl.com/api-contact.html");
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
					var isValidKey = IsValidApiKey(Options.ApiKey);
					if (!isValidKey) return;
					DialogResult = true;
					Close();
				}
				else
				{
					ValidationBlock.Text = "Api Key is required.";
					ValidationBlock.Visibility = Visibility.Visible;
				}
			}
		}

		private bool  IsValidApiKey(string apiKey)
		{
			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetAsync($"https://api.deepl.com/v1/usage?auth_key={apiKey}").Result;
				if (response.IsSuccessStatusCode) return true;
				ValidationBlock.Visibility = Visibility.Visible;
				ValidationBlock.Text = response.StatusCode == HttpStatusCode.Forbidden
					? "Authorization failed. Please supply a valid API Key."
					: $"{response.StatusCode}";
				return false;
			}
		}
	}
}