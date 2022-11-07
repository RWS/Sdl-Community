using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GoogleTranslatorProvider.Interfaces;
using System.Xml.Linq;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using GoogleTranslatorProvider.ViewModels;
using GoogleTranslatorProvider.Views;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class MTEnhancedSettingsAction : AbstractTellMeAction
	{
		public MTEnhancedSettingsAction()
		{
			Name = "MT Enhanced Provider options";
		}

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			if (currentProject == null)
			{
				MessageBox.Show(@"No project is set as active");
			}
			else
			{
				var settings = currentProject.GetTranslationProviderConfiguration();
				if (!settings.Entries.Any(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider")))
				{
					MessageBox.Show(
						@"MT Enhanced Provider is not set on this project\nPlease set it in project settings before using TellMe to access it");
				}
				else
				{
					var translationProvider = settings.Entries.FirstOrDefault(entry =>
						entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider"));

					if (translationProvider != null)
					{
						var mtTranslationOptions =
							new GTPTranslationOptions(translationProvider.MainTranslationProvider.Uri);
						var dialogService = new OpenFileDialogService();

						var settingsControlVm = new SettingsControlViewModel(mtTranslationOptions, dialogService, true);
						var mainWindowVm = new MainWindowViewModel(mtTranslationOptions, settingsControlVm, true);

						var mainWindow = new MainWindow
						{
							DataContext = mainWindowVm
						};

						mainWindowVm.CloseEventRaised += () =>
						{
							settings.Entries.Find(entry =>
									entry.MainTranslationProvider.Uri.ToString().Contains("mtenhancedprovider"))
								.MainTranslationProvider
								.Uri = mtTranslationOptions.Uri;

							currentProject.UpdateTranslationProviderConfiguration(settings);
							mainWindow.Close();
						};

						mainWindow.ShowDialog();
					}
				}
			}
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Settings;
	}
}
