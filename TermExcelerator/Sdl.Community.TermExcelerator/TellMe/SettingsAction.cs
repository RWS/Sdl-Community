using System.Drawing;
using Sdl.Community.TermExcelerator.TellMe.WarningWindow;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			if (project is null)
				ShowWarningMessage();
			else
			{
				var settingsDialog = new Settings();
				settingsDialog.ShowDialog();
			}
		}

		private void ShowWarningMessage()
			{
				var settingsWarningWindow =
					new SettingsActionWarning("https://appstore.rws.com/Plugin/59?tab=documentation");
				settingsWarningWindow.ShowDialog();
			}
		}
	}