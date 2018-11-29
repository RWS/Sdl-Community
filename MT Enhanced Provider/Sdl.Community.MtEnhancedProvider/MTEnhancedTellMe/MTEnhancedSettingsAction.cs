using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	public class MTEnhancedSettingsAction : AbstractTellMeAction
	{
		public MTEnhancedSettingsAction()
		{
			Name = "MTEnhanced Provider options";
		}

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();

			var translationProvider = settings.Entries.FirstOrDefault(entry =>
				entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider"));

			if (translationProvider != null)
			{
				var mtTranslationOptions = new MtTranslationOptions(translationProvider.MainTranslationProvider.Uri);

				var dialog = new MtProviderConfDialog(mtTranslationOptions, true);
				dialog.ShowDialog();

				if (dialog.DialogResult == DialogResult.OK)
				{
					settings.Entries.Find(entry =>
							entry.MainTranslationProvider.Uri.ToString().Contains("mtenhancedprovider"))
						.MainTranslationProvider
						.Uri = mtTranslationOptions.Uri;

					currentProject.UpdateTranslationProviderConfiguration(settings);
				}
			}
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Settings;
	}
}
