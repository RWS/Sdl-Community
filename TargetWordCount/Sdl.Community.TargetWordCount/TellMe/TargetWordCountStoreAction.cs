using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	public class TargetWordCountStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Target word count results";
		public override Icon Icon => PluginResources.Download;

		public TargetWordCountStoreAction()
		{
			Name = "Download Target word count plugin from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/target-word-count/965/");
		}
	}
}