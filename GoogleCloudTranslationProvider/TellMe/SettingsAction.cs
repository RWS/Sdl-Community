using System.Collections.Generic;
using System.Linq;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.View;
using GoogleCloudTranslationProvider.ViewModel;
using GoogleCloudTranslationProvider.ViewModels;
using GoogleCloudTranslationProvider.Views;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = {"project", "settings" };
		private static readonly bool _isAvailable = true;

		public SettingsAction() : base($"{PluginResources.Plugin_Name} Settings", PluginResources.Settings, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

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
			var providerViewModel = new MainWindowViewModel(selectedTranslationOptions, null, languagePairs, true);
			var providerView = new MainWindowView() { DataContext = providerViewModel };
			providerViewModel.CloseEventRaised += providerView.Close;
			providerView.ShowDialog();

			if (!providerViewModel.DialogResult)
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
			AppInitializer.TranslationOptions ??= new Dictionary<string, ITranslationOptions>();
			foreach (var entry in translationProviderConfiguration.Entries)
			{
				var translationProviderReference = entry.MainTranslationProvider;
				if (!IsGoogleCloudProvider(translationProviderReference)
				 || !TryExtractTranslationOptions(translationProviderReference.State, out var translationOptions)
				 || !AppInitializer.TranslationOptions.ContainsKey(translationOptions.Id))
				{
					continue;
				}

				yield return AppInitializer.TranslationOptions[translationOptions.Id];
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
			new SettingsActionWarningView("https://appstore.rws.com/Plugin/174?tab=documentation").ShowDialog();
		}

		private static bool IsGoogleCloudProvider(TranslationProviderReference translationProviderReference)
		{
			return translationProviderReference is not null
				&& translationProviderReference.Uri.Scheme.EndsWith(Constants.GoogleTranslationScheme);
		}
	}
}