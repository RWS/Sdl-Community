using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Studio.TellMe.View;
using LanguageWeaverProvider.Studio.TellMe.ViewModel;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.Studio.TellMe
{
	internal class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "project", "settings" };
		private static readonly string _actionName = Constants.TellMe_Settings_Name;
		private static readonly Icon _icon = PluginResources.TellMe_Settings;
		private static readonly bool _isAvailable = true;

		public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

		private static void ShowDialog()
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			if (projectController is null || projectController.CurrentProject is null)
			{
				DisplayNoProviderAvailableWarning();
				return;
			}

			var translationProviderConfiguration = projectController.CurrentProject.GetTranslationProviderConfiguration();
			var translationOptionsList = UpdateAndRetrieveGoogleCloudTranslationOptions(translationProviderConfiguration);
			var selectedTranslationOptions = SelectCurrentProvider(translationOptionsList);
			if (selectedTranslationOptions is null)
			{
				return;
			}

			var languagePairs = GetLanguagePairs(projectController);
            var providerViewModel = new PairMappingViewModel(selectedTranslationOptions, languagePairs.ToArray());
			var providerView = new PairMappingView() { DataContext = providerViewModel };
			providerViewModel.CloseEventRaised += providerView.Close;
			providerView.ShowDialog();

			if (!providerViewModel.SaveChanges)
			{
				return;
			}

			var currentTranslationProviderConfigurationEntry = translationProviderConfiguration.Entries.FirstOrDefault(x => x.MainTranslationProvider.Uri.AbsoluteUri.Equals(selectedTranslationOptions.Uri.AbsoluteUri));
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

		private static ITranslationOptions SelectCurrentProvider(IEnumerable<ITranslationOptions> translationOptionsList)
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

		private static IEnumerable<ITranslationOptions> UpdateAndRetrieveGoogleCloudTranslationOptions(TranslationProviderConfiguration translationProviderConfiguration)
		{
			ApplicationInitializer.TranslationOptions ??= new Dictionary<string, ITranslationOptions>();
			foreach (var entry in translationProviderConfiguration.Entries)
			{
				var translationProviderReference = entry.MainTranslationProvider;
				if (!IsGoogleCloudProvider(translationProviderReference)
				 || !TryExtractTranslationOptions(translationProviderReference.State, out var translationOptions)
				 || !ApplicationInitializer.TranslationOptions.ContainsKey(translationOptions.Id))
				{
					continue;
				}

				yield return ApplicationInitializer.TranslationOptions[translationOptions.Id];
			}
		}

		private static bool TryExtractTranslationOptions(string state, out ITranslationOptions translationOptions)
		{
			try
			{
				translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(state);
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
			new SettingsActionWarning(Constants.TellMe_Documentation_Url).ShowDialog();
		}

		private static bool IsGoogleCloudProvider(TranslationProviderReference translationProviderReference)
		{
			return translationProviderReference is not null
				&& translationProviderReference.Uri.Scheme.StartsWith(Constants.BaseTranslationScheme);
		}
	}
}