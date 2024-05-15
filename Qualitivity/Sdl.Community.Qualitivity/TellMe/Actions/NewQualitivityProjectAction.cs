using System.Drawing;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.WarningWindow;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.TellMe.Actions
{
	public class NewQualitivityProjectAction : AbstractTellMeAction
	{
		public NewQualitivityProjectAction() => Name = $"{PluginResources.Plugin_Name} New Project";

		public override string Category =>
			string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.QualitivityCreateProjectAction_Icon;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			if (!SdlTradosStudio.Application.GetAction<QualitivityContextCreateQualitivityProject>().Enabled && !SdlTradosStudio.Application.GetAction<QualitivityProfessionalCreateQualitivityProject>().Enabled)
			{
				ShowInfo();
				return;
			}

			ApplicationContext.QualitivityViewController.NewTimeTrackerProject();
		}

		private void ShowInfo()
		{
			var infoWindow =
				new SettingsActionWarning(PluginResources.SettingsAction_ProjectAlreadyQualitivity,
					PluginResources.SettingsAction_Title_NewQualitivityProject);

			infoWindow.ShowDialog();
		}
	}
}