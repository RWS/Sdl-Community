using Sdl.TellMe.ProviderApi;

namespace SdlXliffToolkit.TellMe
{
	public abstract class ToolkitAbastractTellMeAction : AbstractTellMeAction
	{
		public override string Category =>
			string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override bool IsAvailable => true;
	}
}