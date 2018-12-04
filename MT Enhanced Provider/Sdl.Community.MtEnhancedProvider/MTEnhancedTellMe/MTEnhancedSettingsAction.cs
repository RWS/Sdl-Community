using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	public class MTEnhancedSettingsAction : AbstractTellMeAction
	{
		public MTEnhancedSettingsAction()
		{
			Name = "MT Enhanced Provider options";
		}

		public override void Execute()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			if (projectsController.CurrentProject == null)
			{
				MessageBox.Show("No project is set as active");
			}
			else
			{
				var currentProject = projectsController.CurrentProject;
				var settings = currentProject.GetTranslationProviderConfiguration();

				if (settings.Entries.Count == 0)
				{
					MessageBox.Show(
						"MT Enhanced Provider is not set on this project\nPlease set it in project settings before using TellMe to access it");
				}
				else
				{
					var translationProvider = settings.Entries.FirstOrDefault(entry =>
						entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider"));

					if (translationProvider != null)
					{
						var mtTranslationOptions =
							new MtTranslationOptions(translationProvider.MainTranslationProvider.Uri);

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
			}
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Settings;
	}
}
