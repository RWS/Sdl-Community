using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.ViewModels;
using GoogleTranslatorProvider.Views;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace GoogleTranslatorProvider.TellMe
{
	// TODO: If we keep this class, add the errors to PluginResources
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{Constants.GoogleNaming_FullName} - Settings";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Settings;

		public override string Category => Constants.GoogleNaming_FullName;

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			if (currentProject is null)
			{
				MessageBox.Show("No project is set as active");
				return;
			}

			var settings = currentProject.GetTranslationProviderConfiguration();
			if (!settings.Entries.Any(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains(Constants.GoogleTranslationScheme)))
			{
				MessageBox.Show($"{Constants.GoogleNaming_FullName} is not set on this project\nPlease set it in project settings before using TellMe to access it");
				return;
			}

			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains(Constants.GoogleTranslatorString.Replace(" ", "+")));
			if (translationProvider is null)
			{
				return;
			}

			var translationOptions = new GCTPTranslationOptions(translationProvider.MainTranslationProvider.Uri);
			var settingsViewModel = new SettingsViewModel(translationOptions, true);
			var mainWindowViewModel = new MainWindowViewModel(translationOptions, settingsViewModel, true);
			var mainWindow = new MainWindowView { DataContext = mainWindowViewModel };
			mainWindowViewModel.CloseEventRaised += () =>
			{
				translationProvider.MainTranslationProvider.Uri = translationOptions.Uri;
				currentProject.UpdateTranslationProviderConfiguration(settings);
				currentProject.Save();
				mainWindow.Close();
			};

			mainWindow.ShowDialog();
		}
	}
}