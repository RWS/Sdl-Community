using System.Drawing;
using System.Linq;
using System.Xml;
using Sdl.Community.DeepLMTProvider.WPF;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLSettingsAction: AbstractTellMeAction
	{
		public DeepLSettingsAction()
		{
			Name = "DeepL options";
		}
		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();

			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider"));
			if (translationProvider != null)
			{
				var uri = translationProvider.MainTranslationProvider.Uri;
				var options = new DeepLTranslationOptions(uri);
				var dialog = new DeepLWindow(options, true);
				dialog.ShowDialog();
				if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
				{
					settings.Entries
						.Find(entry =>
							entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider"))
						.MainTranslationProvider.Uri = options.Uri;

					currentProject.UpdateTranslationProviderConfiguration(settings);
				}
			}
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";

		public override Icon Icon => PluginResources.Settings;
	}
}
