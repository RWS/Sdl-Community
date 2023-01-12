using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = $"{Constants.MicrosoftNaming_FullName} - AppStore";
		}

		public override bool IsAvailable => true;

		public override string Category => $"{Constants.MicrosoftNaming_FullName}";

		public override Icon Icon => PluginResources.Download;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_StoreUrl);
		}
	}
}