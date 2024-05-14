using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffCompare.TellMe.Actions
{
	public class SdlXliffCompareAction : AbstractTellMeAction
	{
		public SdlXliffCompareAction()
		{
			Name = $"{PluginResources.Plugin_Name}";
		}

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.SDLXLIFFCompare;
		public override bool IsAvailable => true;

		public override void Execute() => ApplicationContext.CompareAction();
	}
}