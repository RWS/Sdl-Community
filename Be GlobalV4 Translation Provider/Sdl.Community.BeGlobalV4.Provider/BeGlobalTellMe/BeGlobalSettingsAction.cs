using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	public class BeGlobalSettingsAction : AbstractTellMeAction
	{
		public BeGlobalSettingsAction()
		{
			Name = "SDLMachineTranslationCloud options";
		}

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application?.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject?.GetTranslationProviderConfiguration();
			var translationProvider = settings?.Entries?.FirstOrDefault(entry =>
				entry.MainTranslationProvider.Uri.OriginalString.Contains("sdlmachinetranslationcloudprovider"));
			if (translationProvider != null)
			{
				var uri = translationProvider.MainTranslationProvider?.Uri;
				var languagePairs = GetProjectLanguagePairs(currentProject);
				var options = new BeGlobalTranslationOptions(uri);
				var beGlobalVm = new BeGlobalWindowViewModel(options, languagePairs, null);
				var beGlobalWindow = new BeGlobalWindow();
				beGlobalWindow.DataContext = beGlobalVm;
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					settings.Entries
						.Find(entry =>
							entry.MainTranslationProvider.Uri.OriginalString.Contains("sdlmachinetranslationcloudprovider"))
						.MainTranslationProvider.Uri = options.Uri;

					currentProject.UpdateTranslationProviderConfiguration(settings);
				}
			}
		}

		private LanguagePair[] GetProjectLanguagePairs(FileBasedProject currentProject)
		{
			var languagePairs = new List<LanguagePair>();
			if (currentProject == null) return languagePairs.ToArray();

			var sourceLanguage = currentProject.GetProjectInfo()?.SourceLanguage;
			var targetLanguages = currentProject.GetProjectInfo().TargetLanguages;
			if (sourceLanguage == null || !targetLanguages.Any()) return languagePairs.ToArray();

			foreach (var targetLanguage in targetLanguages)
			{
				var languagePair = new LanguagePair(sourceLanguage.CultureInfo, targetLanguage.CultureInfo);
				languagePairs.Add(languagePair);
			}
			return languagePairs.ToArray();
		}


		public override bool IsAvailable => true;
		public override string Category => "Machine Translation Cloud results";

		public override Icon Icon => PluginResources.Settings;
	}
}
