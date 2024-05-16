using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe.View;
using Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe
{
    class SettingsAction : TellMeAction
	{
		static readonly string[] _helpKeywords = { "project", "settings" };
		static readonly string _actionName = Constants.TellMe_Settings_Name;
		static readonly Icon _icon = PluginResources.TellMe_Settings;
		static readonly bool _isAvailable = true;

		static IDictionary<string, TranslationProviderCascadeEntry> AmazonEntries { get; set; }

        public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

		private static void ShowDialog()
		{
			AmazonEntries = new Dictionary<string, TranslationProviderCascadeEntry>();
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			if (projectController is null || projectController.CurrentProject is null)
			{
				DisplayNoProviderAvailableWarning();
				return;
			}

			var translationProviderConfiguration = projectController.CurrentProject.GetTranslationProviderConfiguration();
			var translationOptionsList = UpdateAndRetrieveAmazonTranslationOptions(translationProviderConfiguration);
			var selectedTranslationOptions = SelectCurrentProvider(translationOptionsList);
			if (selectedTranslationOptions is null)
			{
				return;
			}

			var languagePairs = GetLanguagePairs(projectController);
            var dialog = new MtProviderConfDialog(selectedTranslationOptions, ApplicationInitializer.CredentialStore);
			dialog.ShowDialog();

            if (dialog.DialogResult != DialogResult.OK)
			{
				return;
			}

			var currentTranslationProviderConfigurationEntry = AmazonEntries[selectedTranslationOptions.Id];
			currentTranslationProviderConfigurationEntry.MainTranslationProvider.State = JsonConvert.SerializeObject(selectedTranslationOptions);

			projectController.CurrentProject.UpdateTranslationProviderConfiguration(translationProviderConfiguration);
		}

        private static IEnumerable<LanguagePair> GetLanguagePairs(ProjectsController projectsController)
		{
			var projectInfo = projectsController.CurrentProject.GetProjectInfo();
			var sourceLanguage = projectInfo.SourceLanguage;
			for (var i = 0; i < projectInfo.TargetLanguages.Length; i++)
			{
				var targetLanguage = projectInfo.TargetLanguages[i];
				var languagePair = new LanguagePair(sourceLanguage, targetLanguage);
				yield return languagePair;
			}
		}

		private static TranslationOptions SelectCurrentProvider(IEnumerable<TranslationOptions> translationOptionsList)
		{
			switch (translationOptionsList.Count())
			{
				case 0:
					DisplayNoProviderAvailableWarning();
					return null;
				case 1:
					return translationOptionsList.First();
			}
			
			var viewModel = new ProviderSelectorViewModel(translationOptionsList);
			var view = new ProviderSelectorView() { DataContext = viewModel };
			viewModel.CloseEventRaised += view.Close;
			view.ShowDialog();

			return viewModel.SelectedProvider;
		}

		private static IEnumerable<TranslationOptions> UpdateAndRetrieveAmazonTranslationOptions(TranslationProviderConfiguration translationProviderConfiguration)
		{
			ApplicationInitializer.TranslationOptions ??= new Dictionary<string, TranslationOptions>();
			foreach (var entry in translationProviderConfiguration.Entries)
			{
				var translationProviderReference = entry.MainTranslationProvider;
				if (!IsAmazonProvider(translationProviderReference)
				 || !TryExtractTranslationOptions(translationProviderReference, out var translationOptions)
				 || !ApplicationInitializer.TranslationOptions.ContainsKey(translationOptions.Id))
				{
					continue;
				}

				AmazonEntries[translationOptions.Id] = entry;
				yield return ApplicationInitializer.TranslationOptions[translationOptions.Id];
			}
		}

		private static bool TryExtractTranslationOptions(TranslationProviderReference translationProviderReference, out TranslationOptions translationOptions)
		{
			try
			{
				translationOptions = new TranslationOptions(translationProviderReference.Uri);
                return true;
			}
			catch
			{
				translationOptions = null;
				return false;
			}
		}


		private static void DisplayNoProviderAvailableWarning()
		{
			SystemSounds.Beep.Play();
			new SettingsActionWarningView(Constants.TellMe_Documentation_Url).ShowDialog();
		}

		private static bool IsAmazonProvider(TranslationProviderReference translationProviderReference)
		{
			return translationProviderReference is not null
				&& translationProviderReference.Uri.Scheme.StartsWith(Constants.BaseTranslationScheme);
        }
	}
}