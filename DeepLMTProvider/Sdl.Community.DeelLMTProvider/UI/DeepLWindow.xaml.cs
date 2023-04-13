using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.UI
{
	public partial class DeepLWindow
	{
		private readonly DeepLTranslationProviderConnecter _connecter;
		private readonly bool _isTellMeAction;
		private readonly LanguagePair[] _languagePairs;

		public DeepLWindow(DeepLTranslationOptions options, TranslationProviderCredential credentialStore = null,
			LanguagePair[] languagePairs = null, bool isTellMeAction = false)
		{
			InitializeComponent();
			_languagePairs = languagePairs;
			_isTellMeAction = isTellMeAction;

			Formality.SelectedIndex = (int)options.Formality;
			PlainText.IsChecked = options.SendPlainText;
			Options = options;

			PasswordChangedTimer.Elapsed += OnPasswordChanged;

			SetSettingsOnWindow(credentialStore, isTellMeAction);
		}

		public DeepLTranslationOptions Options { get; }

		private Timer PasswordChangedTimer { get; } = new()
		{
			Interval = 500,
			AutoReset = false
		};

		private static void AskUserToRestart()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var documentsOpened = editorController.GetDocuments().Any();

			if (documentsOpened)
			{
				MessageBox.Show(PluginResources.SettingsUpdated_ReopenFilesForEditing,
					PluginResources.SettingsUpdated, MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordChangedTimer.Enabled = true;
		}

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start("https://www.deepl.com/api-contact.html");
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			DeepLTranslationProviderConnecter.ApiKey = Options.ApiKey;
			SetApiKeyValidityLabel();

			Enum.TryParse<Formality>(Formality.SelectedIndex.ToString(), out var formality);
			Options.Formality = formality;

			if (PlainText.IsChecked != null)
			{
				Options.SendPlainText = (bool)PlainText.IsChecked;
			}

			if (_isTellMeAction)
			{
				AskUserToRestart();
			}
			else if (ValidationBlock.Visibility == Visibility.Visible)
			{
				return;
			}

			DialogResult = true;
			Close();
		}

		private void OnPasswordChanged(object sender, EventArgs e)
		{
			Application.Current.Dispatcher?.Invoke(() =>
			{
				Options.ApiKey = ApiKeyBox.Password.Trim();

				DeepLTranslationProviderConnecter.ApiKey = Options.ApiKey;
			});

			SetApiKeyValidityLabel();
			SetFormalityCompatibilityLabel();
		}

		private void SetApiKeyValidityLabel()
		{
			if (!string.IsNullOrEmpty(Options.ApiKey))
			{
				SetValidationBlockMessage(Visibility.Collapsed);

				var isApiKeyValidResponse = DeepLTranslationProviderConnecter.IsApiKeyValidResponse;
				if (isApiKeyValidResponse?.IsSuccessStatusCode ?? false)
					return;

				SetValidationBlockMessage(Visibility.Visible, isApiKeyValidResponse?.StatusCode == HttpStatusCode.Forbidden
					? "Authorization failed. Please supply a valid API Key."
					: $"{isApiKeyValidResponse?.StatusCode}");

				return;
			}

			SetValidationBlockMessage(Visibility.Visible, PluginResources.ApiKeyIsRequired_ValidationBlockMessage);
		}

		private void SetFormalityCompatibilityLabel()
		{
			var currentLanguagePairs = _isTellMeAction
							? Options?.LanguagesSupported?.Keys.Select(key => new CultureInfo(key)).ToList()
							: _languagePairs?.Select(lp => new CultureInfo(lp.TargetCultureName)).ToList();

			Application.Current.Dispatcher.Invoke(() =>
			{
				var formalityIncompatibleLanguages =
					DeepLTranslationProviderConnecter.GetFormalityIncompatibleLanguages(currentLanguagePairs);

				if (formalityIncompatibleLanguages.Count > 0)
				{
					NotCompatibleBlock.Visibility = Visibility.Visible;
					NotCompatibleStackPanel.ToolTip = new ToolTip
					{
						Content = $"{string.Join(Environment.NewLine, formalityIncompatibleLanguages)}"
					};
				}
				else
				{
					NotCompatibleBlock.Visibility = Visibility.Collapsed;
				}
			});
		}

		private void SetSettingsOnWindow(TranslationProviderCredential credentialStore, bool isTellMeAction)
		{
			if (isTellMeAction)
			{
				ApiKeyBox.IsEnabled = false;
			}
			else
			{
				if (credentialStore == null)
					return;

				ApiKeyBox.Password = credentialStore.Credential;
				Options.ApiKey = ApiKeyBox.Password;
			}
		}

		private void SetValidationBlockMessage(Visibility visibility, string message = null)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ValidationBlock.Visibility = visibility;
				ValidationBlock.Text = message;
			});
		}
	}
}