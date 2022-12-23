using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = $"{Constants.GoogleNaming_FullName} - AppStore";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Download;

		public override string Category => Constants.GoogleNaming_FullName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_StoreUrl);
		}
	}
}