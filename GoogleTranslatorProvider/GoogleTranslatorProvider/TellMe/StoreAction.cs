using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = $"{Constants.GooglePluginName} - AppStore";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Download;

		public override string Category => Constants.GooglePluginName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_StoreUrl);
		}
	}
}