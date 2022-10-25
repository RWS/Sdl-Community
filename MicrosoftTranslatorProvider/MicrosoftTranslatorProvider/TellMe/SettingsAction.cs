using System;
using System.Drawing;
using Sdl.TellMe.ProviderApi;
using System.Linq;
using System.Windows;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Studio.TranslationProvider;
using MicrosoftTranslatorProvider.View;
using MicrosoftTranslatorProvider.ViewModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = "MT Enhanced Provider options";
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Settings;

		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			if (currentProject is null)
			{
				MessageBox.Show("No project is set as active");
				return;
			}

			var settings = currentProject.GetTranslationProviderConfiguration();
			if (!settings.Entries.Any(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider")))
			{
				MessageBox.Show("MT Enhanced Provider is not set on this project\nPlease set it in project settings before using TellMe to access it");
				return;
			}

			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("mtenhancedprovider"));
			if (translationProvider == null)
			{
				return;
			}

			var translationOptions = new MTETranslationOptions(translationProvider.MainTranslationProvider.Uri);
			var dialogService = new OpenFileDialogService();
			var settingsControlViewModel = new SettingsControlViewModel(translationOptions, dialogService, true);
			var mainWindowViewModel = new MainWindowViewModel(translationOptions, settingsControlViewModel, true);
			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			mainWindowViewModel.CloseEventRaised += () =>
			{
				settings.Entries
						.Find(entry => entry.MainTranslationProvider.Uri.ToString().Contains("mtenhancedprovider"))
						.MainTranslationProvider
						.Uri = translationOptions.Uri;

				currentProject.UpdateTranslationProviderConfiguration(settings);
				mainWindow.Close();
			};

			mainWindow.ShowDialog();
		}
	}
}