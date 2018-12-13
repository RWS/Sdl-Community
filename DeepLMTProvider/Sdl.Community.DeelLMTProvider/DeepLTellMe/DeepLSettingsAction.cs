using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
			Name = "DeepL MT Provider options";
		}
		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			if (currentProject == null)
			{
				MessageBox.Show("No project is set as active");
			}
			else
			{
				var settings = currentProject.GetTranslationProviderConfiguration();

				if (!settings.Entries.Any(entry =>
					entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider")))
				{
					MessageBox.Show(
						"DeepL is not set on this project\nPlease set it in project settings before using TellMe to access it");
				}
				else
				{
					var translationProvider = settings.Entries.FirstOrDefault(entry =>
						entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider"));
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
									entry.MainTranslationProvider.Uri.OriginalString.Contains(
										"deepltranslationprovider"))
								.MainTranslationProvider.Uri = options.Uri;

							currentProject.UpdateTranslationProviderConfiguration(settings);
						}
					}
				}
			}
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";

		public override Icon Icon => PluginResources.Settings;
	}
}
