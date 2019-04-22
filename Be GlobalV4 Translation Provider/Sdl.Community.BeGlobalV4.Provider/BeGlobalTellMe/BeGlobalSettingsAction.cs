using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	public class BeGlobalSettingsAction	  : AbstractTellMeAction
	{
		public BeGlobalSettingsAction()
		{
			Name = "BeGlobal options";
		}
		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();
			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("beglobaltranslationprovider"));
			if (translationProvider != null)
			{
				//var uri = translationProvider.MainTranslationProvider.Uri;
				// var options = new BeGlobalTranslationOptions(uri);
				//var beGlobalWindow =   new BeGlobalWindow();
				//var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, options,true);
				//beGlobalWindow.DataContext = beGlobalVm;
				//beGlobalWindow.ShowDialog();
				//if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				//{
				//	settings.Entries
				//		.Find(entry =>
				//			entry.MainTranslationProvider.Uri.OriginalString.Contains("beglobaltranslationprovider"))
				//		.MainTranslationProvider.Uri = options.Uri;

				//	currentProject.UpdateTranslationProviderConfiguration(settings);
				//}
			}
		}

		public override bool IsAvailable => true;
		public override string Category => "BeGlobal results";

		public override Icon Icon => PluginResources.Settings;
	}
}
