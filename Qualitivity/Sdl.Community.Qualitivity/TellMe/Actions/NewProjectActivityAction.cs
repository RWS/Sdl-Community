using System.Drawing;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.WarningWindow;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.TellMe.Actions
{
	public class NewProjectActivityAction : AbstractTellMeAction
	{
		public NewProjectActivityAction() => Name = $"{PluginResources.Plugin_Name} New Project Activity";

		public override string Category =>
			string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.QualitivityCreateProjectTaskAction_Icon;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			ApplicationContext.QualitivityViewController.Activate();

			if (!SdlTradosStudio.Application.GetAction<QualitivityCreateProjectTaskAction>().Enabled)
			{
				ShowInfo();
				return;
			}

			SdlTradosStudio.Application.ExecuteAction<QualitivityCreateProjectTaskAction>();
		}

		private void ShowInfo()
		{
			var infoWindow =
				new SettingsActionWarning(PluginResources.SettingsAction_NoQualitivityProjectSelected,
					PluginResources.Qualitivity_NewProjectActivityTitle);
			infoWindow.ShowDialog();
		}
	}
}