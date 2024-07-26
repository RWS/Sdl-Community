using System.Diagnostics;
using System.Drawing;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin.TellMe
{
	public class AmazonMTProviderStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Amazon Translate MT Provider results";
		public override Icon Icon => PluginResources.Download;

		public AmazonMTProviderStoreAction()
		{
			Name = "Download the Amazon Translate MT Provider from the AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/18");
		}
	}
}
