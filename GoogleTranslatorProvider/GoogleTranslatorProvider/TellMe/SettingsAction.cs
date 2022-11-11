using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using GoogleTranslatorProvider.ViewModels;
using GoogleTranslatorProvider.Views;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{Constants.GooglePluginName} - Settings";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Settings;

		public override string Category => Constants.GooglePluginName;

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			if (currentProject is null)
			{
				MessageBox.Show(@"No project is set as active");
				return;
			}

			var settings = currentProject.GetTranslationProviderConfiguration();
			if (!settings.Entries.Any(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains(Constants.GoogleTranslationScheme)))
			{
				MessageBox.Show($"{Constants.GooglePluginName} is not set on this project\nPlease set it in project settings before using TellMe to access it");
				return;
			}

			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains(Constants.GoogleTranslatorString.Replace(" ", "+")));
			if (translationProvider is null)
			{
				return;
			}

			var translationOptions = new GTPTranslationOptions(translationProvider.MainTranslationProvider.Uri);
			var settingsControlViewModel = new SettingsControlViewModel(translationOptions, new OpenFileDialogService(), true);
			var mainWindowViewModel = new MainWindowViewModel(translationOptions, settingsControlViewModel, true);
			var mainWindow = new MainWindow { DataContext = mainWindowViewModel };

			mainWindowViewModel.CloseEventRaised += () =>
			{
				settings.Entries
						.Find(entry => entry.MainTranslationProvider.Uri.ToString().Contains(Constants.GoogleTranslationScheme))
						.MainTranslationProvider.Uri = translationOptions.Uri;
				currentProject.UpdateTranslationProviderConfiguration(settings);
				mainWindow.Close();
			};

			mainWindow.ShowDialog();
		}
	}
}
