using System.Drawing;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace SdlXliffToolkit.TellMe.Actions
{
	public class ToolkitAction : ToolkitAbastractTellMeAction
	{
		public ToolkitAction()
		{
			Name = $"{PluginResources.Plugin_Name}";
		}

		public override Icon Icon => PluginResources.toolkit__128;

		public override void Execute() =>
			SdlTradosStudio.Application?.GetAction<SdlToolkitFilesViewPartAction>().ExecuteAction();
	}
}